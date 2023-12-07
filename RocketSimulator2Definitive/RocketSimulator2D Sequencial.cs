//Matheus Veras Soares
//Daniel Marto da Silva

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class Rocket
{
    public string nome { get; set; } //nome do foguete
    public double massRocket { get; set; } //massa do foguete sem combustível
    public double massFuel { get; set; }//massa do combustível
    public double posY { get; set; }//posição do foguete no eixo y
    public double taxFuel { get; set; }
    public double massFinalRocket { get; set; }
    public double empuxoMáximo { get; set; }
    public double empuxoAtual { get; set; }
    public double velocidade { get; set; }

    public Rocket(string Nome, double MR, double MF, double Fuel, double MaxE)
    {
        nome = Nome;
        massRocket = MR;
        massFuel = MF;
        posY = 0;
        taxFuel = Fuel;
        empuxoMáximo = MaxE;
    }
}

class Planeta
{
    public string nome { get; set; }
    public double gravidade { get; set; }
    public double raio { get; set; }
 

    public Planeta(string Nome, double G, double R)
    {
        nome = Nome;
        gravidade = G;
        raio = R;
    }
}

class Program
{
    static void Main()
    {
        int time = 0;
        var perfomanceMeasure = new Stopwatch(); //instância do cronômetro
        perfomanceMeasure.Start();//inicio do cronômetro
        
        List<Rocket> rockets = new List<Rocket>()
        {
            new Rocket("Saturn V", 1710000, 2829000, 0.1 * Math.Pow(10,5),33.375 *Math.Pow(10,3)),
            new Rocket("Space Shuttle",78000, 2040000, 5775, 5777 * Math.Pow(10, 3))  //declara nome, peso sem combustível, peso do combustível, taxa de fluxo de massa, empuxo máximo
        };
        List<Planeta> planetas = new List<Planeta>()
        {
            new Planeta("Terra", 9.8, 6370 * Math.Pow(10, 3)),//declara nome, gravidade, raio
            new Planeta("Marte", 3.72, 3389 * Math.Pow(10,3))
        };

        foreach (Rocket rocket in rockets)
        {
            definirRazaoMassa(rocket); //define a massa total do foguete, com base no valor do combustivel pendente
        }

        foreach (Rocket rocket in rockets)
        {
            definirEmpuxoAtual(rocket); //define empuxo máximo que é a força que faz o foguete subir
        }
        Console.WriteLine("SIMULADOR DE FOGUETE 2D");
        Console.WriteLine("-----------------------");
        Console.WriteLine("Selecione seu Foguete:");
        for(int i = 0; i < rockets.Count; i++)
        {
           Console.WriteLine("{0} -  {1}", i+1, rockets[i].nome);
        }
        Console.WriteLine();
        int currentRocket = int.Parse(Console.ReadLine()) - 1;
        Console.WriteLine("Foguete Selecionado:" + rockets[currentRocket].nome);

        Console.WriteLine();
        
        
        Console.WriteLine("Selecione o planeta de simulação:");
        for(int j = 0; j < planetas.Count; j++)
        {
            Console.WriteLine("{0} -  {1}", j+1, planetas[j].nome);
        }
        Console.WriteLine();
        int currentPlanet = int.Parse(Console.ReadLine()) - 1;
        Console.WriteLine("Planeta Selecionado:" + planetas[currentPlanet].nome);
        Console.WriteLine();
        perfomanceMeasure.Start();//inicio do cronômetro
        
        while (time < 10 && rockets[currentRocket].posY < planetas[currentPlanet].raio)//enquanto o tempo for menor que "x" segundos e o foguete não sair da órbita do planeta
        {
            Console.WriteLine("Tempo: {0} segundos", time); //Tempo
            Console.WriteLine("Posição do Foguete: {0}", rockets[currentRocket].posY); //Diz a posição atual do Foguete
            Console.WriteLine("Combustível restante: {0} kg", rockets[currentRocket].massFuel);
            AtualizarStatus(rockets[currentRocket], planetas[currentPlanet]);
            TimeCount();
            ExecutarMúsica();
            if (rockets[currentRocket].posY >= planetas[currentPlanet].raio)//se a posição do foguete for maior que o raio da terra
            {
                Console.WriteLine("O Foguete saiu do Planeta com sucesso!");
            }
            definirRazaoMassa(rockets[currentRocket]);//Fazer a atualização da quantidade de massa do foguete em cada segundo
            definirEmpuxoAtual(rockets[currentRocket]);//Fazer a atualização do empuxo do foguete (porque a mudança de massa influencia no empuxo)
            Console.WriteLine();
            
            time++;
        }
        perfomanceMeasure.Stop();
        Console.WriteLine($"Tempo decorrido: {perfomanceMeasure.Elapsed}");
    }

    static void definirRazaoMassa(Rocket rocket)
    {
        rocket.massFinalRocket = rocket.massRocket + rocket.massFuel;//Massa total = massa foguete + massa combustível
        Thread.Sleep(500);
    }

    static void definirEmpuxoAtual(Rocket rocket)
    {
        rocket.empuxoAtual = Math.Min(rocket.empuxoMáximo, rocket.massFuel / rocket.taxFuel); //Retorna o menor valor entre o empuxo máximo ou a razão entre a massa do combustivel pela taxa de fluxo de massa
        Thread.Sleep(500);
    }

    static double aceleracao(Rocket rocket, Planeta planeta)
    {
        return (rocket.empuxoAtual - rocket.massFinalRocket * planeta.gravidade) / rocket.massFinalRocket; //retorna a aceleração
        //empuxo - peso = empuxo - massa * gravidade do planeta
    }
    
    static void ExecutarMúsica()
    {
        Console.WriteLine("Essa função executa a música do jogo de fundo");
        Thread.Sleep(500);
    }
    
    static void TimeCount()
    {
        Console.WriteLine("Essa Função mostra a contagem do tempo na tela");
        Thread.Sleep(500);
    }
    
    public static void AtualizarStatus(Rocket rocket, Planeta planeta)
    {
        rocket.posY += Math.Abs(rocket.velocidade); //A posição do foguete atualiza com a velocidade atual
        rocket.velocidade += aceleracao(rocket, planeta);//A velocidade é aumentada pelo aumento de aceleracao
        rocket.massFuel -= rocket.taxFuel;//A cada segundo o combustivel vai diminuindo, subtraindo a massa pela taxa de fluxo de massa
        Thread.Sleep(500);
    }
}
