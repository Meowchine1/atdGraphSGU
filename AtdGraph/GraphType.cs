﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace AtdGraph
{
  
    public class GraphType
    {   
        private bool _orientation = false; 
        private bool _weighted = false;
        private Dictionary<string, Dictionary<string, double>> _graph = new Dictionary<string,
            Dictionary<string, double>>();

        public bool Orientation 
        {
            get { return _orientation; } 
            set { _orientation = value; } 
        }
        public bool Weighted
        { 
            get { return _weighted; }    
            set { _weighted = value; } 
        }
        public Dictionary<string, Dictionary<string, double>> Graph
        {
            get { return _graph; } 
        }

        #region CONSTRUCRORS
        public GraphType() { }
        public GraphType(string FilePath)
        {
            using (StreamReader InputFile = new StreamReader(FilePath))
            {
                string tempLine = InputFile.ReadLine();
                _orientation = tempLine.Contains(">") ? true : false;
                _weighted = tempLine.Contains("$") ? true : false;
                int i_step = _weighted ? 2 : 1; // для дополнительного шага в цикле к переменной вес из файла

                while (!InputFile.EndOfStream)
                {
                    tempLine = InputFile.ReadLine();
                    string[] mas = tempLine.Split(' ');
                    string data = mas[0];
                    Dictionary<string, double> adjacency = new Dictionary<string, double>();

                    for (int i = 1; i < mas.Length; i += i_step)
                    {
                        string adj = mas[i];
                        adjacency.Add(adj, _weighted ? double.Parse(mas[i + 1]) : 1); //value
                    }
                    _graph.Add(data, adjacency);
                }
            }

        }

        public GraphType(GraphType g) // конструктор копирования
        {
            _weighted = g._weighted;
            _orientation = g._orientation;
            _graph = new Dictionary<string, Dictionary<string, double>>();

            foreach (var elem in g._graph)
            {
                _graph.Add(elem.Key, elem.Value);   //new Dictionary<T, double>(v.Value))
            }

        }
        #endregion

        #region METHODS

        public void AddTop(string top)
        {           
            if (_graph.ContainsKey(top))
            {
                throw new Exception("Top already exist. Operation doesnt execute.");
            }
            else 
            {
                 Dictionary<string, double> adj = new Dictionary<string, double>();
                _graph.Add(top, adj);
            }
        }

        public void DeleteTop(string top)
        {         
            if (_graph.ContainsKey(top))
            {
                foreach (var elem in _graph)
                {
                    if (elem.Value.ContainsKey(top))
                    {
                        elem.Value.Remove(top);
                    }
                }
                foreach (var elem in _graph)
                {
                    if (elem.Key == top)
                        _graph.Remove(elem.Key);
                    break;
                }
            }
            else
            {
                throw new Exception("Top unexist. Operation doesnt execute.");
            }
        }

        public void AddEdge(string startTop, string endTop, double weight)
        {           
            bool start_exist = _graph.ContainsKey(startTop);
            bool end_exist = _graph.ContainsKey(endTop);

            if (!end_exist & !start_exist)
            {
                throw new Exception("Tops are not exist");
            }
            else
            {
                if (!start_exist)
                {
                    throw new Exception("First top isnt exist");
                }
                else
                {
                    if (!end_exist)
                    {
                        throw new Exception("Second top isnt exist");
                    }
                    else
                    {
                        _graph[startTop].Add(endTop, _weighted ? weight : 0);

                        if (!_orientation)
                        {
                            _graph[endTop].Add(startTop, _weighted ? weight : 0);
                        }
                    }
                }
            }
        }

        public void AddEdge(string startTop, string endTop)
        {
            
            bool start_exist = _graph.ContainsKey(startTop);
            bool end_exist = _graph.ContainsKey(endTop);

            if (!end_exist & !start_exist)
            {
                throw new Exception("Tops are not exist");
            }
            else
            {
                if (!end_exist)
                {
                    throw new Exception("First top isnt exist");
                }
                else
                {
                    if (!start_exist)
                    {
                        throw new Exception("Second top isnt exist");
                    }
                    else
                    {
                        _graph[startTop].Add(endTop, 0);

                        if (!_orientation)
                        {
                            _graph[endTop].Add(startTop, 0);
                        }
                    }
                }
            }           
        }

        public void DeleteEdge(string startTop, string endTop)
        {
           
            bool start_exist = _graph.ContainsKey(startTop);
            bool end_exist = _graph.ContainsKey(endTop);

            if (!end_exist & !start_exist)
            {
                throw new Exception("Tops are not exist");
            }
            else
            {
                if (!end_exist)
                {
                    throw new Exception("First top isnt exist");
                }
                else
                {
                    if (!start_exist)
                    {
                        throw new Exception("Second top isnt exist");
                    }
                    else
                    {
                        foreach (var elem in _graph[startTop].Keys)
                        {
                            if (elem == endTop)
                            {
                                _graph[startTop].Remove(endTop);
                                if (_orientation)
                                {
                                    _graph[endTop].Remove(startTop);
                                }
                                break;
                            }
                        }
                    }
                }
            }                 
        }

        public void WriteFile(string FilePath)
        {
            using (StreamWriter OutputFile = new StreamWriter(FilePath, true))
            {
                OutputFile.WriteLine(this);
            }
        }

        public override string ToString()
        {
            string str = (_orientation ? ">" : "") + (_weighted ? "$\n" : "\n");
            int i = 0;

            foreach (var elem in _graph)
            {
                str += $"{elem.Key}";
                if (elem.Value.Count > 0)
                {
                    str += " ";
                    foreach (var adj in elem.Value)
                    {
                        i++;
                        if (i < elem.Value.Count)
                        {

                            if (_weighted)
                            {
                                str += $"{adj.Key} ${adj.Value} ";
                            }
                            else
                            {
                                str += $"{adj.Key} ";
                            }
                        }
                        else
                        {
                            if (_weighted)
                            {
                                str += $"{adj.Key} ${adj.Value}\n";
                            }
                            else
                            {
                                str += $"{adj.Key}\n";
                            }

                        }
                    }
                }
                else
                {
                    str += "\n";
                }

                i = 0;
            }

            return str;
        }

        // 1a 2) Вывести полустепень исхода данной вершины орграфа.
        public int GetIshodDegree(string node)
        {
            if (_graph.ContainsKey(node))
                return _graph[node].Count;
            else
            {
                throw new Exception("Top is unexist. Operation doesnt complete");
            }
        }

        // 1a 17) Определить, можно ли попасть из вершины u в вершину v через одну какую-либо вершину орграфа.Вывести такую вершину.
        public List<string> CanGetNodeThroughOneNode(string start, string end)
        {
            List<string> nodes = new List<string>();
            foreach (var elem in _graph[start])
            {

                if (_graph[elem.Key].ContainsKey(end))
                {

                    nodes.Add(elem.Key);
                }
            }
            if (nodes.Count == 0)
            {
                throw new Exception("Нельзя дойти через одну вершину");
                
            }
            else
            {
                return nodes;
            }

        
        }

        //1b  7) Построить орграф, являющийся объединением двух заданных.
        public void Obedinenie(GraphType graph)
        {
            foreach (var elem in graph._graph)
            {
                if (this._graph.ContainsKey(elem.Key))// если вершина уже есть в графе
                {
                    foreach (var el in graph._graph[elem.Key])// добавление adjencies добавляемого графа  this.графу 
                    {
                        if (!this._graph[elem.Key].ContainsKey(el.Key) && this._graph.ContainsKey(el.Key))// если нет в. в списке смежности
                            // но вершина  сущетсвует
                        {
                            this._graph[elem.Key].Add(el.Key, el.Value);
                        }
                        else//если не сущ вершина добавим
                            if (!this._graph[elem.Key].ContainsKey(el.Key) && !this._graph.ContainsKey(el.Key))
                        {
                            Dictionary<string, double> adj = new Dictionary<string, double>();
                            this._graph.Add(el.Key,adj);
                            this._graph[elem.Key].Add(el.Key, el.Value);
                        }
                    }
                }
                else// если нет вершины в графе
                {
                    this._graph.Add(elem.Key, elem.Value);
                    /*foreach (var adj in graph._graph[elem.Key])
                    {
                        if (this._graph.ContainsKey(adj.Key))
                        {
                            this._graph[elem.Key].Add(adj.Key, adj.Value);
                        }
                        else
                        {
                            Dictionary<string, double> Ladj = new Dictionary<string, double>();
                            this._graph.Add(adj.Key, Ladj);
                            this._graph[elem.Key].Add(adj.Key, adj.Value);
                        }                
                    }*/

                }
            
            }
        
        }
   
        #endregion


    }

}