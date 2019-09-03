using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public class Node<T>// : IEnumerable
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

	// Move out
	// public int MaxTreeDepth() => MaxDepthFrom(root);
	// public int MaxDepthFrom(Node<T> node)
	// {
	// 	int c = 0;
	// 	foreach (var n in node.children)
	// 		c = max(MaxDepthFrom(n), c);
	// 	return c + 1;
	// }

	// public int MaxTreeChildrenCount() => MaxChildrenCountFrom(root);
	// public int MaxChildrenCountFrom(Node<T> node)
	// {
	// 	int c = children.Count;
	// 	foreach (var n in node.children)
	// 		c = max(MaxChildrenCountFrom(n), c);
	// 	return c;
	// }
	// Out move
}
