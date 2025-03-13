using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Plane: MonoBehaviour
    {
        private int _indexColor;
        private PlaneData _data;
        
        public void SetData(float x, float y, int type)
        {
            _data = new PlaneData(x, y, type);
            transform.position = new Vector3(x, 0f, y);
            if (type == 1)
            {
                gameObject.AddComponent<MovingPlatform>();
            } else if (type == 2)
            {
                gameObject.AddComponent<TeleportPortal>();
                GameController.Instance.AddToTeleportList(this);
            } 
        }
        
        public void SetColor(Material material, int index)
        {
            if (_data.type != 2)
            {
                _indexColor = index;
                GetComponent<MeshRenderer>().material = material;
            }
        }

        public int GetIndexColor()
        {
            return _indexColor;
        }
    }

    public class PlaneData
    {
        public float x;
        public float y;
        public int type; //Moving or teleport or goal 

        public PlaneData(float x, float y, int type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
            
        }
    }
}