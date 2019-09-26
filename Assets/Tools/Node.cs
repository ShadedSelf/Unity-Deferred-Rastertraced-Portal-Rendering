using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public class Node<T>// : IEnumerable?
{
	public List<Node<T>> children = new List<Node<T>>();
	public Node<T> parent;
	public T data;

	public Node<T> root => (parent == null) ? this : parent.root;

	public Node() { }
	public Node(Node<T> parent, T data)
	{
		this.data = data;
		this.parent = parent;
	}

	public void Insert(T data) => Insert(new Node<T>(this, data));
	public void Insert(Node<T> node)
	{
		children.Add(node);
		node.parent = this;
	}
}
