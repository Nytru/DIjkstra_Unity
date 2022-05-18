using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class Node : MonoBehaviour
    {
        public bool visited = false;

        private List<(long path, float weight)> _ps = new List<(long path, float weight)> ();

        public List<(long path, float weight)> AwaliblePath { get => _ps; set => _ps = value; }


        public (List<long> lightestId, float weight) pin;

        public string Name { get; set; }

        public long Id { get; set; }
    }
}
