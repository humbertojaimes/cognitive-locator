﻿using System;
using System.Globalization;
using CognitiveLocator.Domain;
using Xamarin.Forms;

namespace CognitiveLocator.Pages.Converters
{
    public class CountryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Catalogs.GetCountryValue((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}