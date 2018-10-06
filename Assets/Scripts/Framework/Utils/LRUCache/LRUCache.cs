using System;
using System.Collections.Generic;

namespace AW.Cache {

    public class DoubleLinkedListNode<T> {
        public T Value { get; set; }

        public DoubleLinkedListNode<T> Next { get; set; }

        public DoubleLinkedListNode<T> Prior { get; set; }

        public DoubleLinkedListNode(T t) { Value = t; }

        public DoubleLinkedListNode() { }

        public void RemoveSelf() {
            if(Prior != null) Prior.Next = Next;
            if(Next != null) Next.Prior = Prior;
        }

    }

    public class DoubleLinkedList<T> {
        protected DoubleLinkedListNode<T> m_Head;
        private DoubleLinkedListNode<T> m_Tail;
        private int m_Count = 0;

        public DoubleLinkedList() {
            m_Head = new DoubleLinkedListNode<T>();
            m_Tail = m_Head;
            m_Count = 0;
        }

        public DoubleLinkedList(T t) : this() {
            m_Head.Next = new DoubleLinkedListNode<T>(t);
            m_Tail = m_Head.Next;
            m_Tail.Prior = m_Head;
        }

        public DoubleLinkedListNode<T> Tail {
            get { return m_Tail; }
        }

        public DoubleLinkedListNode<T> Head {
            get { return m_Head; }
        }

        public DoubleLinkedListNode<T> AddHead(T t) {
            DoubleLinkedListNode<T> insertNode = new DoubleLinkedListNode<T>(t);
            DoubleLinkedListNode<T> currentNode = m_Head;
            insertNode.Prior = null;
            insertNode.Next = currentNode;
            currentNode.Prior = insertNode;
            m_Head = insertNode;

            m_Count ++;
            if(m_Count == 1) {
                m_Tail = m_Head;
            }
            return insertNode;
        }

        public void RemoveTail() {
            if(m_Count > 0) {
                m_Tail = m_Tail.Prior;
                m_Tail.Next = null;
                m_Count --;
            }
        }
    }

    public class LRUCache <K, V> {
        class DictItem {
            public DoubleLinkedListNode<K> Node { get; set; }
            public V Value { get; set; }
        }
        readonly Dictionary<K, DictItem> _dict;
        readonly DoubleLinkedList<K> _queue = new DoubleLinkedList<K>();
        
        private readonly int _max;
        public LRUCache (int capacity, int max) {
            _dict = new Dictionary<K, DictItem>(capacity);
            _max = max;
        }

		public KeyValuePair<K,V>[] Add(K key, V value) {
			KeyValuePair<K,V>[] rm = null;
            lock (this)
            {
                DoubleLinkedListNode<K> v = _queue.AddHead(key);         //O(1)
                _dict[key] = new DictItem() { Node = v, Value = value }; //O(1)
                rm = checkAndTruncate();
            }
            return rm;
        }

		private KeyValuePair<K,V>[] checkAndTruncate() {
			KeyValuePair<K,V>[] rm = null;
            int count = _dict.Count;                                     //O(1)
            if (count > _max) {
                int needRemoveCount = count - _max;
				rm = new KeyValuePair<K,V>[needRemoveCount];

                for (int i = 0; i < needRemoveCount; i++) {
					K k = _queue.Tail.Value;
					rm[i] = new KeyValuePair<K, V>(k, _dict[k].Value)  ;
                    _dict.Remove(_queue.Tail.Value);                     //O(1)
                    _queue.RemoveTail();                                 //O(1)
                }
            }
            return rm;
        }

        public bool Delete(K key) {
			bool exist = false;
            lock (this) {
				exist = _dict.ContainsKey(key);
				if(exist) {
					_dict[key].Node.RemoveSelf();
					_dict.Remove(key); //O(1) 
				}
            }
			return exist;
        }

        public V Get(K key) {
            lock (this) {
                DictItem ret;
                if (_dict.TryGetValue(key, out ret)) {

                    if(ret.Node != _queue.Head) {
                        ret.Node.RemoveSelf();
                        _queue.AddHead(key);
                    }

                    return ret.Value;
                }
                return default(V);
            }
        }

		public KeyValuePair<K,V>[] Clear() {
			KeyValuePair<K,V>[] rm = null;
			int count = _dict.Count; 
			if(count > 0) {
				rm = new KeyValuePair<K,V>[count];

				for (int i = 0; i < count; i++) {
					K k = _queue.Tail.Value;
					rm[i] = new KeyValuePair<K, V>(k, _dict[k].Value)  ;
					_dict.Remove(_queue.Tail.Value);                     //O(1)
					_queue.RemoveTail();                                 //O(1)
				}
			}
			return rm;
		} 

        //不关心返回值
        public bool Touch(K key) {
			bool touch = false;
            lock (this) {
				touch = _dict.ContainsKey(key);
            }
			return touch;
        }

    }

}