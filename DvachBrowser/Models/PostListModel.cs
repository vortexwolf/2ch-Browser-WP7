﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;

namespace DvachBrowser.Models
{
    [DataContract]
    public class PostListModel
    {
        [DataMember(Name = "thread")]
        public PostItemModel[][] Posts { get; set; }
    }
}
