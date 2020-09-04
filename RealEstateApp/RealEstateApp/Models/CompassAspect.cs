using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class CompassAspect
    {
        public string CurrentAspect { get; set; }
        public double RotationAngle { get; set; }
        public double CurrentHeading { get; set; }


    }
}
