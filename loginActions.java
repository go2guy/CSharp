package com.d3.login;

import org.openqa.selenium.*;
import org.openqa.selenium.interactions.Actions;
import org.openqa.selenium.support.*;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.testng.Assert;
import org.openqa.selenium.Keys;

public class loginActions {
	
	public WebDriver Driver;
    public String TimeToWait; 
    public WebDriverWait wait;
    loginMapping mapping = new loginMapping(Driver);
    		
    //Utils utils = new Utils();
   	//Properties p = Utils.loadProperties(".\\conf\\properties.properties");
	//String webdriverTimeout = p.getProperty("WebdriverTimeout");
	//Long timeout = Long.valueOf(webdriverTimeout);
    
    
    public void init(WebDriver driver, Long timeout)
    {

      	mapping = new loginMapping(driver);
        PageFactory.initElements(driver, mapping);
        Driver = driver;
        wait = new WebDriverWait(driver, timeout);
    	
    }
    
    
    public void veriyHomePage(WebDriver driver)
    {
  
        try
        {
            wait.until(ExpectedConditions.titleContains("D3 Banking"));
            Assert.assertTrue(true);
            
        }
        catch(Exception e)
        {
        	//Assert.assertTrue(false);
            throw new NotFoundException("FAIL.");
        }
    }
    
    public void loginUn(WebDriver driver, String username)
    {
    	wait.until(ExpectedConditions.visibilityOf(mapping.UserName));
    	mapping.UserName.sendKeys(username);
    }
    
    public void loginPw(WebDriver driver, String password)
    {
    	wait.until(ExpectedConditions.visibilityOf(mapping.Password));
    	mapping.Password.sendKeys(password);
    }
    
    public void submit(WebDriver driver)
    {
	    wait.until(ExpectedConditions.visibilityOf(mapping.Submit));	
	    new Actions(driver).moveToElement(mapping.Submit).click().perform();
	//     	Actions builder = new Actions(driver); 
	//        builder.click(Mapping.Submit).release().build().perform();
    }
    
    public void secretQuestion(WebDriver driver, String secretq)
    {
	    wait.until(ExpectedConditions.visibilityOf(mapping.SecretQuestion));	
	    mapping.SecretQuestion.sendKeys(secretq);
	}
        
    public void privateDevice(WebDriver driver)
    {
	    wait.until(ExpectedConditions.visibilityOf(mapping.Submit));
	    mapping.PrivateDevice.click();
    }
    
    public void termsOfService(WebDriver driver)
    {
        if (mapping.Terms.isDisplayed() == true){	
        	new Actions(driver).moveToElement(mapping.Terms).sendKeys(Keys.SPACE);
        }
    }

}
