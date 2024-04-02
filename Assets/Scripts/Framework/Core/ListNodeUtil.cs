using System.Collections.Generic;

namespace Framework.Core
{
    public class ListNodeUtil:Singleton<ListNodeUtil>
    {
        public class ListNode
        {
            public int val;
            public ListNode next;

            public ListNode(int val = 0, ListNode next = null)
            {
                this.val = val;
                this.next = next;
            }
        }
        public ListNode GenerateRandomLinkedList(int upperLimit)
        {
            // 创建链表并填充数字
            ListNode head = null;
            ListNode current = null;
            List<int> numbers = new List<int>();

            for (int i = 0; i <= upperLimit; i++)
            {
                numbers.Add(i);
            }

            // 使用 Fisher-Yates 洗牌算法打乱数字顺序
            System.Random random = new System.Random();
            for (int i = numbers.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
            }

            // 将打乱后的数字填充到链表中
            foreach (int number in numbers)
            {
                if (head == null)
                {
                    head = new ListNode(number);
                    current = head;
                }
                else
                {
                    current.next = new ListNode(number);
                    current = current.next;
                }
            }

            return head;
        }
    }
}