using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace REDZONE.AppCode
{
    public class emailObject
    {
            // ============= Public Class Properties DECLARATIONS =============
    MailMessage myEmailMessage = new MailMessage();
    public List<string> emailLog = new List<string>();
    public bool msgIsReady
    {
        get {
            if (String.IsNullOrEmpty(myEmailMessage.From.Address) ) { emailLog.Add("The Sender Email Address is Missing"); }
            if (myEmailMessage.To.Count == 0) { emailLog.Add("Email Message must have at list one Recipient"); }
            if (String.IsNullOrEmpty(myEmailMessage.Subject)) { emailLog.Add("Email Subject Line is Required"); }
            if (String.IsNullOrEmpty(myEmailMessage.Body)) { emailLog.Add("Email Body Text cannot be blank"); }
            if (emailLog.Count > 0 ){ return false; }
            else{ return true;}
       }
    }
    // ============= END OF Public Class Properties DECLARATIONS =============
    // --------- CLASS CONSTRUCTOR -------------------------
	public emailObject() 	{ // Empty Constructor
    }
    public emailObject(string sender, string receiver, string subject, string emailMsgText)
    {
        myEmailMessage = new MailMessage(sender, receiver, subject, emailMsgText);
    }  
    // --------  END OF CLASS CONSTRUCTORS----------------------

    // --------- CLASS FUNCTIONS -------------------------
    public void setSender(string eAddress, string displayName ="") {
        MailAddress sender = new MailAddress(eAddress, displayName);
        myEmailMessage.Sender = sender;
        myEmailMessage.From = sender;
    }

    public void addTO(string eAddress, string displayName = "")
    {
        MailAddress receiver = new MailAddress(eAddress, displayName);
        myEmailMessage.To.Add(receiver);
    }
    public void addCC(string eAddress, string displayName = "")
    {
        MailAddress receiver = new MailAddress(eAddress, displayName);
        myEmailMessage.CC.Add(receiver);
    }
    public void addBCC(string eAddress, string displayName = "")
    {
        MailAddress receiver = new MailAddress(eAddress, displayName);
        myEmailMessage.Bcc.Add(receiver);
    }

    public bool sendEmail()
    {
        try {
            // Verify that the Email Message is Fully Formed
            if (msgIsReady)
            {
                SmtpClient smtp = new SmtpClient("192.168.2.13"); //gmail server
                //// Add credentials if the SMTP server requires them.
                //smtp.Credentials= System.Net.CredentialCache.DefaultNetworkCredentials;
                smtp.Send(myEmailMessage);
                emailLog.Add("Email Message Sent Successfully.");
                return true;
            }
            else { return false; }        
        }
        catch(Exception ex) {
            emailLog.Add(ex.Message);
            return false;
        }
    }
    // --------- END OF CLASS FUNCTIONS -------------------------
    }
}