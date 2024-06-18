package q113

type TreeNode struct {
	Val   int
	Left  *TreeNode
	Right *TreeNode
}

func pathSum(root *TreeNode, targetSum int) [][]int {
	ans := make([][]int, 0)
	goThroughTree(root, []int{}, 0, targetSum, &ans)
	return ans
}

func goThroughTree(root *TreeNode, recorder []int, currentSum int, targetSum int, ans *[][]int) {
	if root == nil {
		return
	}

	// Append current node's value to recorder
	recorder = append(recorder, root.Val)
	currentSum += root.Val

	// Check if current path sums up to targetSum and is a leaf node
	if root.Left == nil && root.Right == nil && currentSum == targetSum {
		// Make a copy of recorder to store in ans
		temp := make([]int, len(recorder))
		copy(temp, recorder)
		*ans = append(*ans, temp)
	}

	// Traverse left subtree
	goThroughTree(root.Left, recorder, currentSum, targetSum, ans)

	// Traverse right subtree
	goThroughTree(root.Right, recorder, currentSum, targetSum, ans)

	// Backtrack: remove current node from recorder (undo the last append)
	recorder = recorder[:len(recorder)-1]
}
