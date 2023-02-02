﻿using System;
using System.Numerics;
using Xamarin.Essentials;

namespace DlrDataApp.Modules.Base.Shared.Services.Sensors
{
    public class Accelerometer
    {
        public event EventHandler<AccelerometerChangedEventArgs> ReadingChanged
        {
            add => Xamarin.Essentials.Accelerometer.ReadingChanged += value;
            remove => Xamarin.Essentials.Accelerometer.ReadingChanged -= value;
        }
        
        public Vector3 Current { get; private set; }
        public float CurrentX => Current.X;
        public float CurrentY => Current.Y;
        public float CurrentZ => Current.Z;
        public float MaxX { get; private set; }
        public float MaxY { get; private set; }
        public float MaxZ { get; private set; }

        public Accelerometer()
        {
            Reset();
        }

        /// <summary>
        /// EventHandler which collects values after each change
        /// </summary>
        public void Reading_Changed(object sender, AccelerometerChangedEventArgs e)
        {
            Current = e.Reading.Acceleration;
            MaxX = Math.Max(MaxX, Math.Abs(CurrentX));
            MaxY = Math.Max(MaxY, Math.Abs(CurrentY));
            MaxZ = Math.Max(MaxZ, Math.Abs(CurrentZ));
        }

        /// <summary>
        /// Resets values.
        /// </summary>
        public void Reset()
        {
            Current = Vector3.Zero;
            MaxX = 0.0F;
            MaxY = 0.0F;
            MaxZ = 0.0F;
        }
    }
}