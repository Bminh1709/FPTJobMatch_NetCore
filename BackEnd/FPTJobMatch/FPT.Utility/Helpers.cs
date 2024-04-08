﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.Utility
{
    public class Helpers
    {
        public static string GetTimeAgoString(DateTime createdAt)
        {
            TimeSpan timeElapsed = DateTime.Now - createdAt;

            if (timeElapsed.TotalMinutes < 1)
            {
                return "Just now";
            }
            else if (timeElapsed.TotalMinutes < 60)
            {
                return $"{(int)timeElapsed.TotalMinutes} minutes ago";
            }
            else if (timeElapsed.TotalHours < 24)
            {
                return $"{(int)timeElapsed.TotalHours} hours ago";
            }
            else if (timeElapsed.TotalDays < 30)
            {
                return $"{(int)timeElapsed.TotalDays} days ago";
            }
            else
            {
                // Format for longer durations (optional)
                return createdAt.ToString("MMMM dd, yyyy");
            }
        }
    }
}
