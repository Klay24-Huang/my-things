# Generated by Selenium IDE
import pytest
import time
import json
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.chrome.service import Service
from webdriver_manager.chrome import ChromeDriverManager
from selenium.webdriver.common.by import By
from selenium.webdriver.common.action_chains import ActionChains
from selenium.webdriver.support import expected_conditions
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.desired_capabilities import DesiredCapabilities

class TestLogin():
  def setup_method(self, method):
    chrome_options = Options()
    chrome_options.add_argument("--ignore-certificate-errors")
    self.driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=chrome_options)
    self.vars = {}
  
  def teardown_method(self, method):
    self.driver.quit()
  
  def wait_for_window(self, timeout = 2):
    time.sleep(round(timeout / 1000))
    wh_now = self.driver.window_handles
    wh_then = self.vars["window_handles"]
    if len(wh_now) > len(wh_then):
      return set(wh_now).difference(set(wh_then)).pop()
  
  def test_longAndDeploy(self):
    self.driver.get("https://cloud.okteto.com/login")
    self.vars["root"] = self.driver.current_window_handle
    self.driver.set_window_size(1552, 832)
    self.vars["window_handles"] = self.driver.window_handles
    self.driver.find_element(By.CSS_SELECTOR, ".Button").click()
    self.vars["win8443"] = self.wait_for_window(2000)
    self.driver.switch_to.window(self.vars["win8443"])
    self.driver.find_element(By.ID, "login_field").click()
    self.driver.find_element(By.ID, "login_field").send_keys("wkus963@gmail.com")
    self.driver.find_element(By.ID, "password").click()
    self.driver.find_element(By.ID, "password").send_keys("800824arkGithub")
    self.driver.find_element(By.ID, "password").send_keys(Keys.ENTER)
    self.driver.find_element(By.ID, "js-oauth-authorize-btn").click()
    time.sleep(5)
    # self.driver.close()
    self.driver.switch_to.window(self.vars["root"])
    self.driver.find_element(By.CSS_SELECTOR, ".ResourceDetailsButtons > .ToolbarButton:nth-child(1)").click()
    self.driver.find_element(By.CSS_SELECTOR, ".Button:nth-child(1) > .button-content").click()
    self.driver.find_element(By.CSS_SELECTOR, ".ResourceListItem:nth-child(2) > .ResourceListItemParent .ResourceItem__Title").click()
    self.driver.find_element(By.CSS_SELECTOR, ".ResourceDetailsButtons > .ToolbarButton:nth-child(1)").click()
    element = self.driver.find_element(By.CSS_SELECTOR, ".Button:nth-child(1) > .button-content")
    actions = ActionChains(self.driver)
    actions.move_to_element(element).perform()
    self.driver.find_element(By.CSS_SELECTOR, ".Button:nth-child(1) > .button-content").click()
    element = self.driver.find_element(By.CSS_SELECTOR, "body")
    actions = ActionChains(self.driver)
    actions.move_to_element(element, 0, 0).perform()
  