using System;

namespace Models
{
    public class Rocket
    {
        public string name { get; } //name do foguete
        public double massRocket { get; } //massa do foguete sem combustível
        public double massFuel { get; }//massa do combustível
        public double posY { get; }//posição do foguete no eixo y
        public double taxFuel { get; }
        public double massFinalRocket { get; }
        public double empuxoMáximo { get; }
        public double empuxoAtual { get; }
        public double velocidade { get; }

        // Initializes a new instance of the Rocket class.
        //
        // Parameters:
        //   name: The name of the
        //   massRocket: The mass of the
        //   massFuel: The mass of the fuel.
        //   taxFuel: The tax of fuel.
        //   empuxoMáximo: The maximum thrust.
        public Rocket(string name, double massRocket, double massFuel, double taxFuel, double empuxoMáximo)
        {
            this.name = name;
            this.massRocket = massRocket;
            this.massFuel = massFuel;
            this.posY = 0;
            this.taxFuel = taxFuel;
            this.empuxoMáximo = empuxoMáximo;

            definirRazaoMassa();
            definirEmpuxoAtual();
        }

        // Calculates the total mass by adding the mass of the rocket to the mass of the fuel.
        public void definirRazaoMassa()
        {
            massFinalRocket = massRocket + massFuel;//Massa total = massa foguete + massa combustível
        }

        // Defines the current thrust.
        public void definirEmpuxoAtual()
        {
            empuxoAtual = Math.Min(empuxoMáximo, massFuel / taxFuel); //Retorna o menor valor entre o empuxo máximo ou a razão entre a massa do combustivel pela taxa de fluxo de massa
        }

        // Calculates the acceleration of a rocket.
        //
        // Parameters:
        //   rocket: The Rocket object representing the rocket.
        //   gravity: The gravitational force acting on the rocket.
        //
        // Returns:
        //   The acceleration of the rocket.
        public double aceleracao(Rocket rocket, double gravity)
        {
            return (empuxoAtual - massFinalRocket * gravity) / massFinalRocket; //retorna a aceleração
                                                                                //empuxo - peso = empuxo - massa * gravity do planeta
        }

        // Updates the status of the rocket.
        //
        // Parameters:
        //   gravity: The gravity affecting the rocket.
        //
        // Returns:
        //   Nothing.
        public static void AtualizarStatus(double gravity)
        {
            posY += Math.Abs(velocidade); //A posição do foguete atualiza com a velocidade atual
            velocidade += aceleracao(rocket, gravity);//A velocidade é aumentada pelo aumento de aceleracao
            massFuel -= taxFuel;//A cada segundo o combustivel vai diminuindo, subtraindo a massa pela taxa de fluxo de massa
        }
    }
}
