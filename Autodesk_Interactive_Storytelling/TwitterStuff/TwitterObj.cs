﻿using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.Common;

namespace Autodesk_Interactive_Storytelling
{
    /* 
     * This class is used to authenticate Twitter with OAuth and create 
     * a TwitterContext object which can be used to access data from Twitter.
     */
    public class TwitterObj
    {
        private TwitterContext twitterCtx; //Instance of Twitter we use

        public TwitterContext TwitterCtx 
        {
            get
            {
                return twitterCtx;
            }
        }

        public async void DoEverything()
        {
            var auth = DoSingleUserAuth();
            await auth.AuthorizeAsync();
            twitterCtx = new TwitterContext(auth);
        }

        static IAuthorizer DoSingleUserAuth()
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"],
                    AccessToken = ConfigurationManager.AppSettings["accessToken"],
                    AccessTokenSecret = ConfigurationManager.AppSettings["accessTokenSecret"]
                }
            }; 

            return auth;
        }

        
    }
}
