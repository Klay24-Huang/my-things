package q1038

import "fmt"

type TreeNode struct {
	Val   int
	Left  *TreeNode
	Right *TreeNode
}

func bstToGst(root *TreeNode) *TreeNode {
	if root == nil {
		return root
	}
	rightVal := goThroughTree(root.Right, 0)
	root.Val += rightVal
	fmt.Printf("root val after go through right tree is %d \n", root.Val)
	goThroughTree(root.Left, root.Val)
	return root
}

func goThroughTree(root *TreeNode, val int) int {
	if root == nil {
		return val
	}

	// originRootVal := root.Val
	rightVal := goThroughTree(root.Right, val)
	root.Val += rightVal
	// if isLeft {
	// 	root.Val += val
	// }
	leftVal := goThroughTree(root.Left, root.Val)

	// no left tree
	if root.Left == nil {
		return root.Val
	}

	// has left tree
	return leftVal
}

// func goThroughTree(root *TreeNode, val int) int {
// 	if root == nil {
// 		return 0
// 	}

// 	// originRootVal := root.Val
// 	rightVal := goThroughTree(root.Right, val)
// 	root.Val += rightVal
// 	// if isLeft {
// 	// 	root.Val += val
// 	// }
// 	goThroughTree(root.Left, root.Val)
// 	// node end
// 	if root.Right == nil && root.Left == nil {
// 		root.Val += val
// 		return root.Val
// 	}

// 	// no left tree
// 	if root.Left == nil {
// 		return root.Val
// 	}

// 	// has left tree
// 	return root.Left.Val

// 	// rootVal := root.Val
// 	// rightVal := goThroughTree(root.Right, val, false)
// 	// root.Val += rightVal
// 	// if isLeft {
// 	// 	root.Val += val
// 	// }
// 	// leftVal := goThroughTree(root.Left, root.Val, true)
// 	// if rootVal == 3 || rootVal == 5 || rootVal == 6 {
// 	// 	fmt.Printf("root is %d, right val is %d, left val is %d, val is %d \n", rootVal, rightVal, leftVal, val)
// 	// }
// 	// return root.Val
// }
