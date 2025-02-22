# PriceChecker

PriceChecker is a Windows Forms application developed in C# that monitors product prices on various e-commerce websites and alerts you when a product's price falls below a specified target value.

## Features

- **Price Monitoring:** Automatically check product prices on supported websites.
- **Target Price Alerts:** Receive a notification when a product's price drops below your desired target.
- **Data Persistence:** Automatically saves and loads your list of monitored products.
- **Inline Editing:** Easily update product URLs and target prices directly within the application.
- **System Tray Integration:** Runs in the background and notifies you via system tray balloon tips.
- **Headless Browser Automation:** Uses Selenium with headless Chrome to retrieve and parse product pages.

## Supported Websites

- Amazon
- NBsklep
- Ceneo
- Morele
- X-kom

> **Note:** Some websites (such as Allegro) might not work due to protection mechanisms (403/CAPTCHA).

## Prerequisites

- **Operating System:** Windows
- **Development Environment:** Visual Studio (or any C# IDE)
- **.NET Framework:** (Version depending on your project configuration)
- **Google Chrome:** Installed on your machine
- **NuGet Packages:** Ensure the following packages are installed:
  - HtmlAgilityPack
  - Selenium.WebDriver
  - Selenium.Chrome.WebDriver

## Installation

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/PriceChecker.git
   ```

2. **Open the Project:**
   - Open the solution in Visual Studio.

3. **Restore NuGet Packages:**
   - Use Visual Studio's Package Manager to restore the required packages.

4. **Build and Run:**
   - Build the solution and run the application.

## Usage

1. **Add a Product:**
   - Enter the product URL in the first text box.
   - Enter your target price in the second text box.
   - Click the **"Добавить" (Add)** button to add the product to the monitoring list.

2. **Edit or Delete Entries:**
   - Directly edit the URL or price in the DataGridView. Changes are saved automatically when you finish editing.
   - To delete an entry, click the **"Удалить" (Delete)** button next to the corresponding row.

3. **Start Monitoring:**
   - Click the **"Начать проверку" (Start Checking)** button. The application will perform an immediate price check and then continue checking at regular intervals.
   - When a product's price drops below the target, you will receive a notification via the system tray.

4. **System Tray Operations:**
   - The application minimizes to the system tray.
   - Double-click the tray icon or select **"Открыть" (Open)** from the context menu to restore the application.
   - Choose **"Выйти" (Exit)** from the tray menu to close the application.

## Code Overview

- **MainForm.cs:**  
  This is the primary form of the application. It contains:
  - **UI Setup:** Initializes controls such as labels, text boxes, buttons, and a DataGridView.
  - **Data Persistence:** Methods to save and load product entries from a text file stored in the user's AppData folder.
  - **Price Checking:** Uses Selenium WebDriver (in headless mode) to fetch HTML from product pages. HtmlAgilityPack is used to parse the HTML and extract price data.
  - **Notifications:** Uses the Windows Forms `NotifyIcon` to show balloon tip notifications when a price drop is detected.
  - **Editing Support:** Listens to DataGridView events to automatically save changes when data is edited.

## Customization

- **Timer Interval:**  
  Adjust the interval for price checks by modifying the timer setup in the `SetupTimer` method.

- **Adding New Sites:**  
  Extend the `GetPriceFromUrl` method to support additional e-commerce sites by adding custom HTML parsing logic.

- **UI Enhancements:**  
  Customize control properties (size, layout, colors) in the `SetupControls` method to tailor the UI to your needs.

## Troubleshooting

- **ChromeDriver Issues:**  
  Make sure the ChromeDriver version matches your installed version of Google Chrome.

- **Notification Problems:**  
  Verify that your system allows tray notifications and that your security settings are not blocking them.

- **HTML Parsing Errors:**  
  If the structure of a supported site changes, update the corresponding HTML parsing logic in the `GetPriceFromUrl` method.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Acknowledgements

- **HtmlAgilityPack:** For robust HTML parsing.
- **Selenium WebDriver:** For browser automation.
- **.NET Community:** For tools and libraries that made this project possible.