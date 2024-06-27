package q1382

import "sort"

type TreeNode struct {
	Val   int
	Left  *TreeNode
	Right *TreeNode
}

func balanceBST(root *TreeNode) *TreeNode {
	row := make([]*TreeNode, 0)
	row = append(row, root)
	recorder := make([]int, 0)
	for len(row) > 0 {
		newRow := make([]*TreeNode, 0)
		for _, node := range row {
			recorder = append(recorder, node.Val)
			if node.Left != nil {
				newRow = append(newRow, node.Left)
			}

			if node.Right != nil {
				newRow = append(newRow, node.Right)
			}
			row = newRow
		}
	}

	sort.Ints(recorder)
	return createTree(recorder)
}

func createTree(nums []int) *TreeNode {
	lenOfNums := len(nums)
	if lenOfNums == 0 {
		return nil
	}

	if lenOfNums == 1 {
		return &TreeNode{
			Val: nums[0],
		}
	}
	index := lenOfNums / 2
	newTree := TreeNode{
		Val: nums[index],
	}
	newTree.Left = createTree(nums[:index])
	newTree.Right = createTree(nums[index+1:])
	return &newTree
}
