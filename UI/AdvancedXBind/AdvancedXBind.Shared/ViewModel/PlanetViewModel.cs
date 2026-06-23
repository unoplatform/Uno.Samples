using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text.Core;

namespace AdvancedXBind.ViewModel
{
    public sealed class PlanetViewModel
    {
        private SolarSystemPlanets planets;
        private Planet planet;
        private Planet earth;

        public Planet Planet
        {
            get { return planet; }
        }

        public SolarSystemPlanets Planets
        {
            get { return planets; }
        }

        public static PlanetViewModel Create(string planetName)
        {
            return new PlanetViewModel(planetName);
        }

        public PlanetViewModel(string planetName)
        {
            planets = new SolarSystemPlanets();
            planet = planets.Find(planetName);
            earth = planets.Find("Earth");
        }

        public string WeightOnPlanet(string weightOnEarthText)
        {
            double weightOnEarth;
            if (!double.TryParse(weightOnEarthText, out weightOnEarth))
                return "0.0";

            
            double massRatio = earth.Mass / planet.Mass;
            double diameterRatio = earth.Diameter / planet.Diameter;
            double weightOnPlanet = weightOnEarth * diameterRatio * diameterRatio / massRatio;
            return weightOnPlanet.ToString("f2");
        }

        public async Task<bool> DisplayPlanetAsync()
        {
            return await Windows.System.Launcher.LaunchUriAsync(new Uri($"https://en.wikipedia.org/wiki/{Planet.Name}"));
        }

        /// <summary>
        /// Example of multi-binding
        /// </summary>
        /// <param name="mass">Mass in KG</param>
        /// <param name="diameter">Diameter in KM</param>
        /// <returns></returns>
        public double CalculateDensity(double mass, double diameter)
        {
            // calculate volume (convert diameter in KM to radius in M)
            double volume = (4 / 3) * Math.PI * Math.Pow(diameter * 500, 3);

            return mass / volume;
        }

        public async Task<string> SimulateLongRunningMethodAsync()
        {
            DateTimeOffset now = DateTimeOffset.Now;
            Stopwatch sw = Stopwatch.StartNew();
			// We cannot use Delay on WASM because of this issue: https://github.com/unoplatform/uno/issues/13370
#if !__WASM__
			await Task.Delay(50);
#endif
			sw.Stop();
            return $"Started {now} Elapsed: {sw.Elapsed}";
        }
    }

    public class SolarSystemPlanets : ObservableCollection<Planet>
    {
        public SolarSystemPlanets()
        {
            this.Add(new Planet("Mercury", 57910000, 4880, 3.30e23));
            this.Add(new Planet("Venus", 108200000, 12103.6, 4.869e24));
            this.Add(new Planet("Earth", 149600000, 12756.3, 5.972e24));
            this.Add(new Planet("Mars", 227940000, 6794, 6.4219e23));
            this.Add(new Planet("Jupiter", 778330000, 142984, 1.900e27));
            this.Add(new Planet("Saturn", 1429400000, 120536, 5.68e26));
            this.Add(new Planet("Uranus", 2870990000, 51118, 8.683e25));
            this.Add(new Planet("Neptune", 4504000000, 49532, 1.0247e26));
            this.Add(new Planet("Pluto", 5913520000, 2274, 1.27e22));
        }

        public Planet Find(string planetName)
        {
            foreach (Planet p in this)
            {
                if (p.Name.Equals(planetName))
                {
                    return p;
                }
            }
            return null;
        }
    }

    public class Planet
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private double orbit;

        public double Orbit
        {
            get { return orbit; }
            set { orbit = value; }
        }

        private double diameter;

        public double Diameter
        {
            get { return diameter; }
            set { diameter = value; }
        }

        private double mass;

        public double Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public Planet(string name, double orbit, double diameter, double mass)
        {
            this.name = name;
            this.orbit = orbit;
            this.diameter = diameter;
            this.mass = mass;
        }
    }
}
