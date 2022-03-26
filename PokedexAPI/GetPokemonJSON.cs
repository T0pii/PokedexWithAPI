using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PokedexAPI
{
    class GetPokemonJSON
    {
        private static List<string> allPkmnStrings = new List<string>();
        private static readonly Task[] tasks = new Task[8];
        /// <summary>
        /// Tableau 2D[8,2] contenant à l'indice [x,] la génération x,
        /// à l'indice [,0] et [,1] la borne inf et sup des <see cref="Pokemon.id"/>,
        /// </summary>
        public static int[,] tabGen = new int[8, 2] {
            {   1, 151 },
            { 152, 251 },
            { 252, 386 },
            { 387, 493 },
            { 494, 649 },
            { 650, 721 },
            { 722, 802 },
            { 803, 898 }
        };

        /// <summary>
        /// Exécute 8 <see cref="Task"/> récupérant simultanément les 8 générations de <see cref="Pokemon"/>
        /// et attend que les 8 <see cref="Tasks"/> aient fini de s'exécuter avant
        /// de convertir les strings en objets
        /// </summary>
        public static void GetAllGenerations()
        {
            Console.WriteLine("- Récupération des données en cours...");
            tasks[0] = Task.Run(() => { GetJSON(tabGen[0, 0], tabGen[0, 1]); });
            tasks[1] = Task.Run(() => { GetJSON(tabGen[1, 0], tabGen[1, 1]); });
            tasks[2] = Task.Run(() => { GetJSON(tabGen[2, 0], tabGen[2, 1]); });
            tasks[3] = Task.Run(() => { GetJSON(tabGen[3, 0], tabGen[3, 1]); });
            tasks[4] = Task.Run(() => { GetJSON(tabGen[4, 0], tabGen[4, 1]); });
            tasks[5] = Task.Run(() => { GetJSON(tabGen[5, 0], tabGen[5, 1]); });
            tasks[6] = Task.Run(() => { GetJSON(tabGen[6, 0], tabGen[6, 1]); });
            tasks[7] = Task.Run(() => { GetJSON(tabGen[7, 0], tabGen[7, 1]); });
            Task.WaitAll(tasks);
            Console.WriteLine("- Données récupérées !\r\n\r\n");
            ConvertAllStringToObject();
        }

        /// <summary>
        /// Télécharge et stocke tous les Pokemon ayant un ID compris entre <paramref name="deb"/> et <paramref name="fin"/>
        /// à partir d'une API JSON dans une liste , et stocke la liste résultante dans <see cref="allPkmnStrings"/>
        /// </summary>
        /// <param name="deb"></param>
        /// <param name="fin"></param>
        public static void GetJSON(int deb, int fin)
        {
            List<string> pkmnsGen = new List<string>();
            using System.Net.WebClient client = new System.Net.WebClient();
            for (int id = deb; id <= fin; id++)
            {
                //On télécharge sous forme de string le pokémon i et on l'ajoute à la liste
                pkmnsGen.Add(client.DownloadString("https://tmare.ndelpech.fr/tps/pokemons/" + id));
            }
            allPkmnStrings.AddRange(pkmnsGen);
        }

        /// <summary>
        /// Désérialise tous les <see cref="string"/> JSON contenus dans <see cref="allPkmnStrings"/>
        /// sous forme de <see cref="Pokemon"/> et trie la liste d'objets résultants par <see cref="Pokemon.id"/>
        /// </summary>
        public static void ConvertAllStringToObject()
        {
            foreach(string pkmn in allPkmnStrings) 
            {
                Pokemon.AddPkmnObjectToList(JsonSerializer.Deserialize<Pokemon>(pkmn));
            }
            Pokemon.SortPkmnsById();
        }
    }
}
