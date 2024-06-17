package q437

type TreeNode struct {
	Val   int
	Left  *TreeNode
	Right *TreeNode
}

func pathSum(root *TreeNode, targetSum int) int {
	ans := 0
	hastSet := make(map[*TreeNode]struct{})
	var findTarget func(tempSum int, root *TreeNode, isTop bool)
	findTarget = func(tempSum int, root *TreeNode, isTop bool) {
		if root == nil {
			return
		}

		// top node
		if isTop {
			if _, exists := hastSet[root]; !exists {
				hastSet[root] = struct{}{}
			} else {
				return
			}
		}

		tempSum += root.Val
		// fmt.Println(tempSum, root.Val)
		// fmt.Println(tempSum)
		if tempSum == targetSum {
			ans++
		}
		findTarget(tempSum, root.Left, false)
		findTarget(0, root.Left, true)
		findTarget(tempSum, root.Right, false)
		findTarget(0, root.Right, true)
	}
	findTarget(0, root, true)
	return ans
}
