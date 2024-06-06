using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal abstract class BinaryTree<T>
{
	public T Value;
	public BinaryTree<T> Parent;
	public BinaryTree<T> ChildA;
	public BinaryTree<T> ChildB;

	public bool HasChildren => ChildA != null || ChildB != null;
}
