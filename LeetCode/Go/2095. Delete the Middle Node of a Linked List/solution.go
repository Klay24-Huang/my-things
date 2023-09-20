/**
 * Definition for singly-linked list.
 */
package solution

type ListNode struct {
	Val  int
	Next *ListNode
}

func deleteMiddle(head *ListNode) *ListNode {
	if head.Next == nil {
		return nil
	}

	copyHead := head
	count := 0
	for copyHead != nil {
		count++
		copyHead = copyHead.Next
	}

	start := head

	target := count / 2
	for i := 0; i <= target; i++ {
		if i == target-1 {
			beforeCut := head
			next := head.Next
			afterCut := next.Next
			beforeCut.Next = afterCut
			break
		}
		head = head.Next
	}
	return start
}
