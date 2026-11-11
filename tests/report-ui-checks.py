"""Run against BE :5168 and `npm start -- --host 127.0.0.1` on :4200.
Requires Python Playwright and Microsoft Edge (or PLAYWRIGHT_BROWSER_PATH).
"""
import json
import os
import sys
from pathlib import Path
from playwright.sync_api import sync_playwright, expect

sys.stdout.reconfigure(encoding="utf-8")


def check(condition, message):
    assert condition, message
    print(f"PASS {message}")


with sync_playwright() as p:
    executable = os.environ.get(
        "PLAYWRIGHT_BROWSER_PATH",
        r"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
    )
    browser = p.chromium.launch(executable_path=executable, headless=True)
    context = browser.new_context(viewport={"width": 1440, "height": 1000}, accept_downloads=True)
    context.add_init_script("""
      window.revokedUrls = [];
      const revoke = URL.revokeObjectURL.bind(URL);
      URL.revokeObjectURL = url => { window.revokedUrls.push(url); revoke(url); };
    """)
    page = context.new_page()
    errors = []
    page.on("pageerror", lambda error: errors.append(str(error)))
    render_requests = []
    page.on("request", lambda request: render_requests.append(request.post_data_json)
            if request.method == "POST" and "/render" in request.url else None)
    page.goto("http://127.0.0.1:4200")
    expect(page.locator("#report")).to_be_enabled()
    check(len(render_requests) == 0, "No automatic PDF generation")
    expect(page.locator("#report")).to_have_value("s11h")
    expect(page.get_by_role("button", name="Tải PDF", exact=True)).to_be_disabled()
    check(page.locator("dx-data-grid").count() == 0, "No data grid")
    for region in ["title", "tableHeader", "tableBody"]:
        for field in ["font", "bold", "italic"]:
            check(page.locator(f"#{region}-{field}").evaluate("s => s.options[s.selectedIndex]?.text") == "Theo mẫu",
                  f"{region} {field} defaults to Theo mẫu")

    output = Path("frontend/dist/checks")
    output.mkdir(parents=True, exist_ok=True)
    page.screenshot(path=str(output / "report-empty.png"), full_page=True)

    page.locator("#title-size").fill("16")
    page.locator("#title-color").fill("#123456")
    page.locator("#tableBody-bold").select_option(label="Không")
    with page.expect_response("**/api/reports/s11h/render", timeout=60000) as pending:
        page.get_by_role("button", name="Tạo báo cáo", exact=True).click()
    response = pending.value
    check(response.status == 200 and response.body().startswith(b"%PDF-"), "S11H actual PDF response")
    expect(page.get_by_role("button", name="Tải PDF", exact=True)).to_be_enabled(timeout=60000)
    check(render_requests[-1] == {"appearance": {"title": {"fontSize": 16, "color": "#123456"}, "tableBody": {"bold": False}}},
          "Only changed properties sent; false remains boolean")
    expect(page.locator("iframe")).to_have_attribute("src", __import__("re").compile(r"^blob:"))
    with page.expect_download() as download:
        page.get_by_role("button", name="Tải PDF", exact=True).click()
    check(download.value.suggested_filename == "S11H.pdf", "Server filename used for download")
    download.value.save_as(str(output / "S11H.pdf"))
    with context.expect_page() as popup:
        page.get_by_role("button", name="Mở PDF để in ↗").click()
    popup.value.close()
    page.locator("#title-size").fill("17")
    expect(page.get_by_role("button", name="Tải PDF", exact=True)).to_be_disabled()
    expect(page.get_by_role("status")).to_contain_text("Cấu hình đã thay đổi")
    page.get_by_role("button", name="Khôi phục theo mẫu").click()
    expect(page.locator("#title-size")).to_have_value("")
    page.locator("#report").select_option("s106h")
    expect(page.locator("iframe")).to_have_count(0)
    for region in ["title", "tableHeader", "tableBody"]:
        expect(page.locator(f"#{region}-font option:checked")).to_have_text("Theo mẫu")
    check(page.evaluate("window.revokedUrls.length") >= 1, "Old Blob URL revoked on report switch")

    page.locator("#title-size").fill("30")
    expect(page.get_by_role("button", name="Tạo báo cáo", exact=True)).to_be_disabled()
    page.locator("#title-size").fill("")
    page.locator("#title-color").fill("invalid")
    expect(page.get_by_role("button", name="Tạo báo cáo", exact=True)).to_be_disabled()
    page.locator("#title-color").fill("")
    with page.expect_response("**/api/reports/s106h/render", timeout=60000) as pending:
        page.get_by_role("button", name="Tạo báo cáo", exact=True).click()
    check(pending.value.status == 200, "S106H actual PDF response")
    expect(page.get_by_role("button", name="Tải PDF", exact=True)).to_be_enabled(timeout=60000)
    page.screenshot(path=str(output / "report-preview.png"), full_page=True)

    # Delayed response, then switch reports: Angular must abort the request.
    held = []
    page.route("**/api/reports/*/render", lambda route: held.append(route))
    page.get_by_role("button", name="Tạo báo cáo", exact=True).click()
    expect(page.get_by_role("button", name="Đang tạo báo cáo…")).to_be_disabled()
    page.locator("#report").select_option("s11h")
    expect(page.get_by_role("button", name="Tạo báo cáo", exact=True)).to_be_enabled()
    expect(page.locator("iframe")).to_have_count(0)
    check(len(held) == 1, "Report switch cancels pending render without displaying stale PDF")
    held[0].abort()
    page.unroute("**/api/reports/*/render")

    page.route("**/api/reports/*/render", lambda route: route.fulfill(
        status=500, content_type="application/problem+json",
        body=json.dumps({"title": "Không thể tạo báo cáo. Vui lòng thử lại."})))
    page.get_by_role("button", name="Tạo báo cáo", exact=True).click()
    expect(page.get_by_role("alert")).to_contain_text("Không thể tạo báo cáo")
    expect(page.get_by_role("button", name="Tạo báo cáo", exact=True)).to_be_enabled()
    page.unroute("**/api/reports/*/render")

    # API contract and validation through the same FE development proxy.
    for report_id, payload, status in [
        ("missing", {}, 404),
        ("s11h", {"appearance": {"title": {"fontSize": 25}}}, 400),
        ("s11h", {"appearance": {"title": {"fontFamily": "Invalid"}}}, 400),
        ("s106h", {"appearance": {"tableBody": {"color": "red"}}}, 400),
        ("s11h", {"appearance": None}, 200),
    ]:
        result = context.request.post(f"http://127.0.0.1:4200/api/reports/{report_id}/render", data=payload)
        check(result.status == status, f"API {report_id} returns {status}")

    page.set_viewport_size({"width": 390, "height": 844})
    check(page.evaluate("document.documentElement.scrollWidth <= window.innerWidth"), "Mobile layout has no horizontal overflow")
    check(not errors, f"No browser runtime errors: {errors}")
    browser.close()
    print("All browser checks passed.")
