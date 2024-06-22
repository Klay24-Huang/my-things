package q889

// type TreeNode struct {
// 	Val   int
// 	Left  *TreeNode
// 	Right *TreeNode
// }

type TreeNode struct {
	Val   int
	Left  *TreeNode
	Right *TreeNode
}

func constructFromPrePost(preorder []int, postorder []int) *TreeNode {
	if len(preorder) == 1 {
		return &TreeNode{Val: preorder[0]}
	}
	tree := TreeNode{Val: preorder[0]}
	i := find(postorder, preorder[1])
	// if i == 0 {
	// 	tree.Left = &TreeNode{Val: postorder[0]}
	// }
	// fmt.Printf("i is %d\n target is %d\n", i, preorder[1])
	// fmt.Println(preorder)
	// fmt.Println(postorder)
	tree.Left = constructFromPrePost(preorder[1:i+2], postorder[0:i+1])
	if i+2 < len(preorder) {
		tree.Right = constructFromPrePost(preorder[i+2:], postorder[i+1:len(postorder)-1])
	}
	return &tree
}

func find(arr []int, target int) int {
	for i, val := range arr {
		if val == target {
			return i
		}
	}
	return -1
}

// func constructFromPrePost(preorder []int, postorder []int) *TreeNode {
//     preIndex, postIndex := 0, 0
//     var dfs func() *TreeNode
//     dfs = func() *TreeNode {
//         root := &TreeNode{
//             Val: preorder[preIndex],
//         }
//         preIndex++
//         if root.Val != postorder[postIndex] {
//             root.Left = dfs()
//         }
//         if root.Val != postorder[postIndex] {
//             root.Right = dfs()
//         }
//         postIndex++
//         return root
//     }
//     return dfs()

// }
