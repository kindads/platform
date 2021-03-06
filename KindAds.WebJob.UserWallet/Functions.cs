﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.Business;
using KindAds.Common.Interfaces;
using KindAds.Negocio;
using KindAds.Business.Helper;
using KindAds.Business.Email;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using KindAds.Azure;
using KindAds.Common;
using KindAds.Common.Models;

namespace KindAds.WebJob.UserWallet
{
    public class Functions
    {
        public ITrace telemetria { set; get; }
        AspNetUserManager aspNetUserManager { set; get; }
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public Functions()
        {
            telemetria = new Trace();
            aspNetUserManager = new AspNetUserManager();
        }

        public void ProcessQueueMessage([QueueTrigger("%azure-queue-accessdata%")] string message, TextWriter log)
        {
            try
            {
                UserWalletManager userWalletManager = new UserWalletManager();
                MailManager mailManager = new MailManager();
                Notification notification = userWalletManager.GetNotification(message);

                var user = aspNetUserManager.GetById(notification.IdUser);
                var wallet = Task.Run(() => NethereumHelper.CreateUserWallet()).Result;

                user.TokenAddress = wallet.blobname;
                user.WalletAddress = wallet.walletaddress;

                aspNetUserManager.Update(user);

                MailMessage mailMessage = new MailMessage()
                {
                    Body = notification.MailContent,
                    Destination = user.Email,
                    Subject = "Confirm your email"
                };
                mailManager.SendAsync(mailMessage);
            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
        }
    }
}
