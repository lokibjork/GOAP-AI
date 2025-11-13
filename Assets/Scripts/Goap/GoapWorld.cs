using System.Collections.Generic;
using UnityEngine;

public sealed class GoapWorld
{
    // Instância singleton
    private static readonly GoapWorld instance = new GoapWorld();
    // Estado global do mundo
    private static WorldStates world;
    // Fila de pacientes
    private static Queue<GameObject> patients;
    // Fila de cubiculos
    private static Queue<GameObject> cubicles;
    // Construtor estático para inicializar o estado do mundo
    static GoapWorld() 
    { 
        world = new WorldStates();
        patients = new Queue<GameObject>();
        cubicles = new Queue<GameObject>();
        // Popula a a fila de cubículos com os objetos na cena
        GameObject[] cubs = GameObject.FindGameObjectsWithTag("Cubicle");
        foreach (GameObject c in cubs) { cubicles.Enqueue(c); }
        // Informa ao mundo a quantidade de cubículos livres
        if (cubicles.Count > 0) { world.AddState("FreeCubicle", cubicles.Count); }
    }
    // Construtor privado para evitar instância externa
    private GoapWorld() { }
    // Propriedade para acessar a instância singleton
    public static GoapWorld Instance { get { return instance; } }
    // Método para acessar o estado global do mundo
    public WorldStates GetWorld() { return world; }

    // Métodos para gerenciar a fila de pacientes
    public void AddPatient(GameObject patient)
    {
        patients.Enqueue(patient);
    }
    public GameObject RemovePatient()
    {
        if (patients.Count <= 0) return null;
        return patients.Dequeue();
    }
    // Gerencia a fila de cubículos
    public void AddCubicle(GameObject cubicle)
    {
        cubicles.Enqueue(cubicle);
    }
    public GameObject RemoveCubicle()
    {
        if (cubicles.Count <= 0) return null;
        return cubicles.Dequeue();
    }
}
