using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFarm.AStar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition; //��������
        public int gCost = 0;   //����Start���ӵľ��룬��ʧֵ
        public int hCost = 0;   //����Target���ӵľ��룬����ֵ
        public int FCost => gCost + hCost;  //��ǰ���ӵ�ֵ
        public bool isObstacle = false; //��ǰ�����Ƿ����ϰ�
        public Node parentNode;

        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;
        }

        public int CompareTo(Node other)
        {
            //�Ƚ�ѡ����͵�Fֵ������-1��0��1
            int result = FCost.CompareTo(other.FCost);
            if (result == 0)
            {
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }
}

