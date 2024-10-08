from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import Select
from selenium.common.exceptions import NoSuchElementException
from selenium.common.exceptions import NoAlertPresentException
import unittest, time, re

class AppDynamicsJob(unittest.TestCase):
    def setUp(self):
        # AppDynamics will automatically override this web driver
        # as documented in https://docs.appdynamics.com/display/PRO44/Write+Your+First+Script
        self.driver = webdriver.Firefox()
        self.driver.implicitly_wait(30)
        self.base_url = "https://www.google.com/"
        self.verificationErrors = []
        self.accept_next_alert = True
    
    def test_app_dynamics_job(self):
        driver = self.driver
        driver.get("https://www.okteto.com/")
        driver.find_element(By.LINK_TEXT, "Login").click()
        driver.get("https://cloud.okteto.com/")
        driver.find_element(By.XPATH,"//div[@id='app']/div/div[2]/div[2]/div/div/div[2]/div[2]/div/div/div/div/div/div/div/div/div/div/div/div[2]/div").click()
        driver.find_element(By.CSS_SELECTOR, "div.ResourceDetailsButtons > div.ToolbarButton.normal > div.ToolbarButton__icon > span.Icon.layout.vertical.center-center.arrowLoopCircle > svg > path.colorable").click()
        driver.find_element(By.XPATH, "(.//*[normalize-space(text()) and normalize-space(.)='Okteto Manifest Path:'])[1]/following::div[3]").click()
        # ERROR: Caught exception [unknown command []]
        # ERROR: Caught exception [unknown command []]
    
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
        # To know more about the difference between verify and assert,
        # visit https://www.seleniumhq.org/docs/06_test_design_considerations.jsp#validating-results
        self.assertEqual([], self.verificationErrors)

if __name__ == "__main__":
    unittest.main()
