//Simulador de Foguete em duas dimensões, promovendo o lançamento vetorial em Y para cima.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Simulators
{
    public class RocketSimulatorWithoutParallelism
    {
        public int time { get; }
        public List<Rocket> rockets { get; }
        public List<Planet> planets { get; }

        public RocketSimulatorWithoutParallelism(List<Rocket> rockets, List<Planet> planets)
        {
            time = 0;
            this.rockets = rockets;
            this.planets = planets;
        }

        public void RunMusic()
        {
            Console.WriteLine("Essa função executa a música do jogo de fundo");
            Thread.Sleep(500);
        }

        public void TimeCount()
        {
            Console.WriteLine("Essa Função mostra a contagem do tempo na tela");
            Thread.Sleep(500);
        }

        public void run()
        {
            while (time < 10 && rockets[0].posY < planetas[0].radius)//enquanto o tempo for menor que "x" segundos e o foguete não sair da órbita do planeta
            {
                Console.WriteLine("Tempo: {0} segundos", time); //Tempo
                Console.WriteLine("Posição do Foguete: {0}", rockets[0].posY); //Diz a posição atual do Foguete
                Console.WriteLine("Combustível restante: {0} kg", rockets[0].massFuel);

                rockets[0].AtualizarStatus(planetas[0].gravity);
                Thread.Sleep(500);

                TimeCount();
                RunMusic();

                if (rockets[0].posY >= planetas[0].radius)//se a posição do foguete for maior que o raio da terra
                {
                    Console.WriteLine("O Foguete saiu do Planeta com sucesso!");
                }

                rockets[0].definirRazaoMassa();//Fazer a atualização da quantidade de massa do foguete em cada segundo
                Thread.Sleep(500);
                rockets[0].definirEmpuxoAtual();//Fazer a atualização do empuxo do foguete (porque a mudança de massa influencia no empuxo)
                Thread.Sleep(500);
                Console.WriteLine();

                time++;
            }
        }
    }
}
