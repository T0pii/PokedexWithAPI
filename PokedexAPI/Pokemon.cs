using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokedexAPI
{

    public class Name
    {
        public string en { get; set; }
        public string fr { get; set; }
    }

    public class Genus
    {
        public string en { get; set; }
        public string fr { get; set; }
    }

    public class Description
    {
        public string en { get; set; }
        public string fr { get; set; }
    }

    public class Stat
    {
        public string name { get; set; }
        public int stat { get; set; }
    }

    public class Pokemon
    {
        /// <summary>
        /// Liste de <see cref="Pokemon"/>
        /// </summary>
        private static List<Pokemon> allPkmnObjects = new List<Pokemon>();


        public int id { get; set; }
        public Name name { get; set; }
        public List<string> types { get; set; }
        public int height { get; set; }
        public int weight { get; set; }
        public Genus genus { get; set; }
        public Description description { get; set; }
        public List<Stat> stats { get; set; }
        public int lastEdit { get; set; }

        /// <summary>
        /// Ajoute le Pokémon <paramref name="pokemon"/> à la fin de la liste <see cref="allPkmnObjects"/>
        /// </summary>
        /// <param name="pokemon"></param>
        public static void AddPkmnObjectToList(Pokemon pokemon)
        {
            allPkmnObjects.Add(pokemon);
        }

        /// <summary>
        /// Trie la liste <see cref="allPkmnObjects"/> par ordre de <see cref="Pokemon.id"/>
        /// </summary>
        public static void SortPkmnsById()
        {
            allPkmnObjects.Sort((pkmn1, pkmn2) => pkmn1.id.CompareTo(pkmn2.id));
        }

        /// <summary>
        /// Affiche tous les <see cref="Pokemon"/> de la liste <paramref name="listePkmn"/> sous forme de tableau à 11 colonnes formaté dans la console
        /// </summary>
        /// <param name="listePkmn"></param>
        private static void ShowListAsTable(List<Pokemon> listePkmn)
        {
            //Initialisation d'une ConsoleTable
            var table = new ConsoleTable("ID", "Nom", "Type(s)", "Poids", "Taille", "HP" , "Attaque", "Défense", "Attaque Spé.", "Défense Spé.", "Vitesse");
            foreach (Pokemon p in listePkmn)
            {
                //Concatenation des deux types du Pokemon en un string si le Pokemon en possède deux
                string types = p.types.Count() > 1 ?
                        (p.types[0] + " - " + p.types[1]) :
                         p.types[0];
                
                //Recuperation de toutes les stats
                List<int> stats = new List<int>();
                foreach (Stat s in p.stats)
                {
                    stats.Add(s.stat);
                }

                //Ajout des donnes du pokemon en tant que ligne du tableau
                table.AddRow(p.id, p.name.fr, types, p.weight, p.height, stats[0], stats[1], stats[2], stats[3], stats[4], stats[5]);
            }
            //Recuperation des moyennes des poids / hauteur des pokemon
            double nbPkmn = listePkmn.Count();
            double moyPoids = Math.Round(listePkmn.Average<Pokemon>(pkmn => pkmn.weight)/10);
            double moyTaille = Math.Round(listePkmn.Average<Pokemon>(pkmn => pkmn.height)/10);

            var tableMoyennes = new ConsoleTable("Nombre de Pokémon", "Poids moyen (kg)", "Taille moyenne (m)");
            tableMoyennes.AddRow(nbPkmn, moyPoids, moyTaille);

            //Affichage des deux tables dans la console
            table.Write(Format.Alternative);
            Console.WriteLine("\r\n");
            tableMoyennes.Write(Format.MarkDown);
            Console.WriteLine("\r\n\r\n");

        }

        /// <summary>
        /// Retourne vrai si le <see cref="Pokemon"/> <paramref name="pokemon"/> appartient à la génération <paramref name="gen"/>
        /// </summary>
        /// <param name="pokemon"></param>
        /// <param name="gen"></param>
        /// <returns>Un booléen équivalent à l'appartenance du <paramref name="pokemon"/> à la génération <paramref name="gen"/></returns>
        public static bool IsPkmnInGen(Pokemon pokemon, int gen)
        {
            int[,] tabGen = GetPokemonJSON.tabGen;
            //On regarde si l'ID du pokémon est compris entre les bornes du tableau tabGen pour la génération choisie
            if ((pokemon.id >= tabGen[gen-1, 0]) && (pokemon.id <= tabGen[gen-1, 1]))
            {
                return true;
            } else
            {
                return false;
            }
        }

        /// <summary>
        /// Affiche tous les <see cref="Pokemon"/> contenus dans <see cref="allPkmnObjects"/>
        /// </summary>
        public static void ShowPokemon()
        {
            ShowListAsTable(allPkmnObjects);
        }

        /// <summary>
        /// Affiche tous les <see cref="Pokemon"/> contenus dans <see cref="allPkmnObjects"/> qui possèdent le type <paramref name="type"/>
        /// </summary>
        /// <param name="type"></param>
        public static void ShowPokemon(string type)
        {
            //Liste contenant tous les pokemons qui ont le type "type"
            List<Pokemon> pkmnsWithType = allPkmnObjects.Where(pokemon => pokemon.types.Contains(type)).ToList();
            ShowListAsTable(pkmnsWithType);
        }

        /// <summary>
        /// Affiche tous les <see cref="Pokemon"/> contenus dans <see cref="allPkmnObjects"/>
        /// et étant de la génération <paramref name="gen"/>
        /// </summary>
        /// <param name="gen"></param>
        public static void ShowPokemon(int gen)
        {
            List<Pokemon> pkmnFromGen = new List<Pokemon>();
            pkmnFromGen = allPkmnObjects.Where(pkmn => IsPkmnInGen(pkmn, gen)).ToList();
            ShowListAsTable(pkmnFromGen);
        }

        /// <summary>
        /// Affiche un <see cref="Pokemon"/> de chaque type par génération ; prend le premier type qui n'a pas déjà été enregistré parmi les 2 types (si le Pokemon en possède deux)
        /// </summary>
        public static void ShowAPkmnPerTypePerGen()
        {
            List<Pokemon> allPkmns = new List<Pokemon>();
            for(int nbGen = 1; nbGen <= 8;nbGen++) {
                //Liste contenant tous les Pokemons de la generation nbGen
                List<Pokemon> entireGen = allPkmnObjects.Where(p => IsPkmnInGen(p,nbGen)).ToList();
                List<Pokemon> pkmnPerGen = new List<Pokemon>(18);
                List<string> pkmnTypes = new List<string>(18);
                //on parcourt la liste et on ajoute le pokemon et son type aux deux listes si le type n'a pas déjà été stocké
                //si le pokemon a 2 types on prend le premier qui est "libre" puis on passe au pokemon suivant
                foreach (Pokemon pkmn in entireGen) {
                    foreach(string type in pkmn.types)
                    {
                        if(!pkmnTypes.Contains(type))
                        {
                            //Si le pokemon a plusieurs types, on supprime son deuxieme type et on definit
                            //le premier type comme etant celui qui a ete trouve comme non contenu dans pkmnTypes
                            //ainsi dans l'affichage final on aura un seul type d'affiché et non deux
                            if(pkmn.types.Count()>1)
                            {
                                pkmn.types[0] = type;
                                pkmn.types.RemoveAt(1);
                            } 
                            pkmnPerGen.Add(pkmn);
                            pkmnTypes.Add(type);
                            break;
                        }
                    }
                }
                //On ajoute les pokémon trouvés à la liste finale
                allPkmns.AddRange(pkmnPerGen);
            }
            ShowListAsTable(allPkmns);
        }
    }
}
