# -*- coding: utf-8 -*-
from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from webdriver_manager.chrome import ChromeDriverManager
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import Select
from selenium.common.exceptions import NoSuchElementException
from selenium.common.exceptions import NoAlertPresentException
# import chromedriver_binary
import unittest, time, re

class DeloyAction(unittest.TestCase):
    def setUp(self):
        chrome_options = Options()
        chrome_options.add_argument("--ignore-certificate-errors")
        # chrome_options.add_argument('--headless')
        # chrome_options.add_argument("start-maximized")
        chrome_options.add_argument("window-size=1920,1080")
        chrome_options.add_experimental_option('excludeSwitches', ['enable-logging'])
        self.chrome_options = chrome_options
        self.driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=chrome_options)
        self.driver.implicitly_wait(30)
        self.base_url = "https://www.google.com/"
        self.verificationErrors = []
        self.accept_next_alert = True
    
    def test_deloy_action(self):
        driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=self.chrome_options)
        driver.get("https://www.okteto.com/")
        # time.sleep(5)
        driver.find_element(By.LINK_TEXT, "Login").click()
        driver.get("https://cloud.okteto.com/")
        driver.find_element(By.XPATH,"//div[@id='app']/div/div[2]/div[2]/div/div/div[2]/div[2]/div/div/div/div/div/div/div/div/div/div/div/div[2]/div").click()
        driver.find_element(By.CSS_SELECTOR, "div.ResourceDetailsButtons > div.ToolbarButton.normal > div.ToolbarButton__icon > span.Icon.layout.vertical.center-center.arrowLoopCircle > svg > path.colorable").click()
        driver.find_element(By.XPATH, "(.//*[normalize-space(text()) and normalize-space(.)='Okteto Manifest Path:'])[1]/following::div[3]").click()
        #ERROR: Caught exception [unknown command []]
        #ERROR: Caught exception [unknown command []]
    
    def is_element_present(self, how, what):
        try: self.driver.find_element(by=how, value=what)
        except NoSuchElementException as e: return False
        return True
    
    def is_alert_present(self):
        try: self.driver.switch_to_alert()
        except NoAlertPresentException as e: return False
        return True
    
    def close_alert_and_get_its_text(self):
        try:
            alert = self.driver.switch_to_alert()
            alert_text = alert.text
            if self.accept_next_alert:
                alert.accept()
            else:
                alert.dismiss()
            return alert_text
        finally: self.accept_next_alert = True
    
    def tearDown(self):
        self.driver.quit()
        self.assertEqual([], self.verificationErrors)

if __name__ == "__main__":
    unittest.main()
