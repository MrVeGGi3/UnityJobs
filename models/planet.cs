using System;

namespace Models
{
    public class Planet
    {
        public string name { get; }
        public double gravity { get; }
        public double radius { get; }

        // Initializes a new instance of the Planet class with the given parameters.
        //
        // Parameters:
        //   name: The name of the planet.
        //   gravity: The gravitational force of the planet.
        //   radius: The radius of the planet.
        //
        // Returns:
        //   A new Planet object.
        public Planet(string name, double gravity, double radius)
        {
            this.name = name;
            this.gravity = gravity;
            this.radius = radius;
        }
    }
}
