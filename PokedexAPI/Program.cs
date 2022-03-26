using System;

namespace PokedexAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            GetPokemonJSON.GetAllGenerations();

            //Affiche tous les Pokémon
            //Pokemon.ShowPokemon();

            //Affiche tous les Pokémon de type Acier (moyennes des données incluses)
            //Pokemon.ShowPokemon("Steel");

            //Affiche tous les Pokémon de la génération 4
            Pokemon.ShowPokemon(4);

            //Affiche un Pokémon de chaque type par génération
            //Pokemon.ShowAPkmnPerTypePerGen();
        }
    }
}
