﻿using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chick_fil_A.Views
{
    public partial class OrderPage : ContentPage
    {
        public OrderPage()
        {
            InitializeComponent();
            BackgroundImageSource = ImageSource.FromResource("Chick-fil-A.Images.Chic-Fil-a-Sandwich-2-1.jpeg");
        }
    }
}