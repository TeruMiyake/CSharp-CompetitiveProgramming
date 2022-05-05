using System;
using System.Collections.Generic;

namespace LibraryForDotnetCore.MySTL
{
    /// <summary>
    /// RBST(RandomizedBinarySearchTree) による平衡二分探索木
    /// 目的1 : Set, Dictionary に 二分探索機能をつける
    /// 目的2 : MultiSet, MultiDictionary の作成
    /// </summary>
    /// <typeparam name="T">IComparable</typeparam>
    public class RandomizedBinarySearchTree<T> where T : IComparable
    {
        private Node root = null;
        static System.Random rnd = new System.Random();
        /// <summary>
        /// RBST 上の各ノードを指すが、実質的には、再帰的な（部分）木である
        /// Node.ReCount : ノードの子を付け替えたあとに呼び出すメソッドで、自身を含めたノードの数をカウントしなおす
        /// </summary>
        public class Node
        {
            readonly public T Value;
            public int TreeSize = 1;
            // RChildTreeSize はコピー負荷を下げるために、保持しない。TreeSize と LChildTreeSize があれば計算できるため。
            public int LChildTreeSize = 0;
            public Node LChild;
            public Node RChild;
            /// <summary>
            /// このコンストラクタは要らないのでは
            /// </summary>
            /// <param name="Value"></param>
            /// <param name="LChild"></param>
            /// <param name="RChild"></param>
            //public Node(T Value, Node LChild, Node RChild)
            //{
            //    this.Value = Value;
            //    this.LChild = LChild;
            //    this.RChild = RChild;
            //    RecountTreeSize();
            //}
            public Node(T Value)
            {
                this.Value = Value;
            }
            public void RecountTreeSize()
            {
                LChildTreeSize = RecountLChildTreeSize();
                TreeSize = LChildTreeSize + RecountRChildTreeSize() + 1;
            }
            /// <summary>
            /// node.LChild.TreeSize としてしまうと、LChild == null の場合に参照エラーとなる
            /// それを防ぐためにこのメソッドを利用する
            /// </summary>
            /// <returns></returns>
            public int RecountLChildTreeSize()
            {
                if (LChild == null) return 0;
                else return LChild.TreeSize;
            }
            /// <summary>
            /// node.RChild.TreeSize としてしまうと、RChild == null の場合に参照エラーとなる
            /// それを防ぐためにこのメソッドを利用する
            /// </summary>
            /// <returns></returns>
            public int RecountRChildTreeSize()
            {
                if (RChild == null) return 0;
                else return RChild.TreeSize;
            }
        }
        public int GetTreeSize()
        {
            return root == null ? 0 : root.TreeSize;
        }
        /// <summary>
        /// Node Merge(Node l, Node r): ∀(l, r)∈(L×R), l < r を満たす RBST l, r に対し、根がランダムに選ばれたという仮定を満たすように再帰的に Merge して RBST を返す
        /// </summary>
        /// <param name="node_l"></param>
        /// <param name="node_r"></param>
        /// <returns></returns>
        static Node Merge(Node node_l, Node node_r)
        {
            if (node_l == null) return node_r;
            else if (node_r == null) return node_l;
            double lsize = node_l.TreeSize;
            double rsize = node_r.TreeSize;
            double sizesum = lsize + rsize;
            // node_l を根とする場合
            // node_l の頂点を根とし、node_l の左部分木をマージ後の左部分木とする
            // マージ後の右部分木は、node_l の右部分木と、node_r を再帰的にマージして決める
            if (lsize / sizesum > rnd.NextDouble())
            {
                node_l.RChild = Merge(node_l.RChild, node_r);
                node_l.RecountTreeSize();
                return node_l;
            }
            // node_r を根とする場合は逆の処理
            else
            {
                node_r.LChild = Merge(node_l, node_r.LChild);
                node_r.RecountTreeSize();
                return node_r;
            }
        }
        /// <summary>
        /// Tuple<Node, Node> </Node>Split(Node t, int k): [0, k) [k, n) へと、どちらも RBST を満たすように再帰的に Split して返す
        /// </summary>
        /// <param name="node"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        static Tuple<Node, Node> Split(Node node, int k)
        {
            if (node == null) return new Tuple<Node, Node>(null, null);
            int size = node.TreeSize;
            //System.Diagnostics.Debug.Assert(k <= size);
            if (k == 0) return new Tuple<Node, Node>(null, node);
            else if (k == size) return new Tuple<Node, Node>(node, null);
            else
            {
                // [0, k) なので結局 <k頂点, size-k頂点> への分割
                int lsize = node.LChildTreeSize;
                // 分割箇所が根より右なので、左側の木の左部分木が確定
                if (lsize + 1 <= k)
                {
                    // node を左側の木とする
                    // node の頂点と左部分木は変わらない
                    Tuple<Node, Node> splitted = Split(node.RChild, k - lsize - 1);
                    // node の右部分木を k で Split したものの左部分木が新しい右部分木
                    node.RChild = splitted.Item1;
                    node.RecountTreeSize();
                    return new Tuple<Node, Node>(node, splitted.Item2);
                }
                // 分割箇所が根より左なので、右側の木の右部分木が確定
                else
                {
                    // node を右側の木とする
                    // node の頂点と右部分木は変わらない
                    Tuple<Node, Node> splitted = Split(node.LChild, k);
                    node.LChild = splitted.Item2;
                    node.RecountTreeSize();
                    return new Tuple<Node, Node>(splitted.Item1, node);
                }
            }

        }
        /// <summary>
        /// [value 未満のノード] [新ノード] [value 以上のノード] となるようにノードを挿入する。
        /// （既に同じ値があっても重複して挿入される。）
        /// </summary>
        /// <param name="value"></param>
        public void Insert(T value)
        {
            int idx = LowerBound(value);
            InsertByIdx(value, idx);
        }
        /// <summary>
        /// [0, idx) [idx, size) として、その間に value を割り込ませる
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="idx"></param>
        private void InsertByIdx(T value, int idx)
        {
            Tuple<Node, Node> splitted = Split(root, idx);
            root = Merge(Merge(splitted.Item1, new Node(value)), splitted.Item2);
        }
        public void Remove(T value)
        {
            int idx = LowerBound(value);
            RemoveByIdx(idx);
        }
        private void RemoveByIdx(int idx)
        {
            Tuple<Node, Node> splitted_l = Split(root, idx);
            Tuple<Node, Node> splitted_r = Split(splitted_l.Item2, 1);
            root = Merge(splitted_l.Item1, splitted_r.Item2);
        }
        /// <summary>
        /// key 以上の要素のうち最も左側の添字位置を返す（存在しなければ size を返す）
        /// ex) k-3, k-2, k-1, (k) ←ココ, k, ...
        /// 木が null のときは 0 を返す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int LowerBound(T key)
        {
            if (root == null) return 0;
            int ng = -1;
            int ok = root.TreeSize;
            // チェックすべきターゲットとなる Node 及びその idx
            Node node = root;
            int idx = root.LChildTreeSize;
            while (ng + 1 < ok)
            {
                T node_value = node.Value;
                // ターゲットがキーよりも小さければ、右部分木に移る
                if (node_value.CompareTo(key) < 0)
                {
                    //System.Diagnostics.Debug.Assert(ng <= idx);
                    ng = idx;
                    // 右部分木があれば、右部分木へ移る
                    // 右部分木の頂点 idx = 親の idx + (右部分木の左部分木の頂点数 + 1)
                    if (node.RChild != null)
                    {
                        node = node.RChild;
                        idx += node.LChild == null ? 1 : node.LChildTreeSize + 1;
                    }
                    // 右部分木が無い時、探索が終了していることが保証される（はず）
                    //else
                    //{
                    //    System.Diagnostics.Debug.Assert(ng + 1 >= ok);
                    //}
                }
                // ターゲットがキー以上ならば、左部分木に移る
                else
                {
                    //System.Diagnostics.Debug.Assert(idx <= ok);
                    ok = idx;
                    // 左部分木へ移る
                    // 左部分木の頂点 idx = 親の idx - (左部分木の右部分木の頂点数 - 1)
                    if (node.LChild != null)
                    {
                        node = node.LChild;
                        idx -= node.RChild == null ? 1 : node.RChild.TreeSize + 1;
                    }
                    // 右部分木が無い時、探索が終了していることが保証される（はず）
                    //else
                    //{
                    //    System.Diagnostics.Debug.Assert(ng + 1 >= ok);
                    //}
                }
            }
            return ok;
        }
        /// <summary>
        /// key より大きい要素のうち最も左側の添字位置を返す（存在しなければ size を返す）
        /// ex) k-3, k-2, k-1, k, k, (k+1)←ココ, k+2, ...
        /// 木が null のときは 0 を返す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int UpperBound(T key)
        {
            if (root == null) return 0;
            int ng = -1;
            int ok = root.TreeSize;
            // チェックすべきターゲットとなる Node 及びその idx
            Node node = root;
            int idx = root.LChild == null ? 0 : root.LChild.TreeSize;
            while (ng + 1 < ok)
            {
                T node_value = node.Value;
                // ターゲットがキー以下なら、右部分木に移る
                if (node_value.CompareTo(key) <= 0)
                {
                    ng = idx;
                    // 右部分木があれば、右部分木へ移る
                    // 右部分木の頂点 idx = 親の idx + (右部分木の左部分木の頂点数 + 1)
                    if (node.RChild != null)
                    {
                        node = node.RChild;
                        idx += node.LChild == null ? 1 : node.LChildTreeSize + 1;
                    }
                    // 右部分木が無い時、探索が終了していることが保証される（はず）
                    //else
                    //{
                    //    System.Diagnostics.Debug.Assert(ng + 1 >= ok);
                    //}
                }
                // ターゲットがキー以上ならば、左部分木に移る
                else
                {
                    ok = idx;
                    // 左部分木へ移る
                    // 左部分木の頂点 idx = 親の idx - (左部分木の右部分木の頂点数 - 1)
                    if (node.LChild != null)
                    {
                        node = node.LChild;
                        idx -= node.RChild == null ? 1 : node.RChild.TreeSize + 1;
                    }
                    // 右部分木が無い時、探索が終了していることが保証される（はず）
                    //else
                    //{
                    //    System.Diagnostics.Debug.Assert(ng + 1 >= ok);
                    //}
                }
            }
            return ok;
        }
        /// <summary>
        /// T key が RBST 内に」存在すれば true, 存在しなければ false を返す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Find(T key)
        {
            if (root == null) return false;
            Node target = root;
            while (target != null)
            {
                // 一致
                if (target.Value.CompareTo(key) == 0)
                {
                    return true;
                }
                // key < target -> 左部分木の頂点をターゲットに
                else if (target.Value.CompareTo(key) > 0)
                {
                    if (target.LChild == null) return false;
                    else
                    {
                        //idx -= target.GetLChildTreeSize();
                        //idx += target.LChild.GetLChildTreeSize();
                        target = target.LChild;
                    }
                }
                // target < key -> 右部分木の頂点をターゲットに
                else
                {
                    if (target.RChild == null) return false;
                    else
                    {
                        //idx += 1;
                        //idx += target.RChild.GetLChildTreeSize();
                        target = target.RChild;
                    }
                }
            }
            return false;
        }
        public T FindByIdx(int idx)
        {
            System.Diagnostics.Debug.Assert(idx < root.TreeSize);
            Node target = root;
            int targetIdx = target.LChildTreeSize;
            while (true)
            {
                // 一致
                if (targetIdx == idx)
                {
                    break;
                }
                // idx < targetIdx -> 左部分木の頂点をターゲットに
                else if (idx < targetIdx)
                {
                    targetIdx -= target.LChildTreeSize;
                    targetIdx += target.LChild.LChildTreeSize;
                    target = target.LChild;
                }
                // targetIdx < idx -> 右部分木の頂点をターゲットに
                else
                {
                    targetIdx += 1;
                    targetIdx += target.RChild.LChildTreeSize;
                    target = target.RChild;
                }
            }
            return target.Value;
        }
        /// <summary>
        /// 最大値を返す。木が null の場合は Assert で弾く
        /// </summary>
        /// <returns></returns>
        public T Max()
        {
            System.Diagnostics.Debug.Assert(root != null);
            Node node = root;
            while (node.RChild != null) node = node.RChild;
            return node.Value;
        }
        /// <summary>
        /// 最小値を返す。木が null の場合は Assert で弾く
        /// </summary>
        /// <returns></returns>
        public T Min()
        {
            System.Diagnostics.Debug.Assert(root != null);
            Node node = root;
            while (node.LChild != null) node = node.LChild;
            return node.Value;
        }
        /// <summary>
        /// 昇順にソートされた List を返す
        /// </summary>
        /// <returns></returns>
        public List<T> GetSortedList()
        {
            List<T> res = new List<T>();
            EnumerateRecursive(root, res);
            return res;
        }
        /// <summary>
        /// 通りがけ順にノードを探索するための再帰メソッド
        /// </summary>
        /// <param name="node"></param>
        /// <param name="list"></param>
        private void EnumerateRecursive(Node node, List<T> list)
        {
            if (node == null) return;
            EnumerateRecursive(node.LChild, list);
            list.Add(node.Value);
            EnumerateRecursive(node.RChild, list);
        }
    }
    /// <summary>
    /// O(logN) で二分探索可能な Set On RBST。
    /// ただし定数倍は重い。2*10^5 くらいでギリギリ。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SetOnRBST<T> where T : IComparable
    {
        protected RandomizedBinarySearchTree<T> rbst = new RandomizedBinarySearchTree<T>();
        /// <summary>
        /// O(logN)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(T value)
        {
            return rbst.Find(value);
        }
        /// <summary>
        /// 集合に要素を挿入する
        /// </summary>
        /// <param name="value"></param>
        /// <returns>要素が見つからず、正常に挿入した場合は true。それ以外の場合は false。</returns>
        public virtual bool Add(T value)
        {
            if (Contains(value))
            {
                return false;
            }
            else
            {
                rbst.Insert(value);
                return true;
            }
        }
        /// <summary>
        /// 集合から要素を削除する
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>要素が見つかり、正常に削除された場合は true。それ以外の場合は false。</returns>
        public bool Remove(T value)
        {
            if (!Contains(value))
            {
                return false;
            }
            else
            {
                rbst.Remove(value);
                return true;
            }
        }
        /// <summary>
        /// key より大きい要素のうち最も左側の添字位置を返す（存在しなければ size を返す）
        /// ex) k-3, k-2, k-1, k, k, (k+1)←ココ, k+2, ...
        /// 木が null のときは 0 を返す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int LowerBound(T key)
        {
            return rbst.LowerBound(key);
        }
        /// <summary>
        /// key より大きい要素のうち最も左側の添字位置を返す（存在しなければ size を返す）
        /// ex) k-3, k-2, k-1, k, k, (k+1)←ココ, k+2, ...
        /// 木が null のときは 0 を返す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int UpperBound(T key)
        {
            return rbst.UpperBound(key);
        }
        /// <summary>
        /// 集合に含まれる要素を昇順でソートしたリストを返す
        /// </summary>
        /// <returns></returns>
        public List<T> GetSortedList()
        {
            return rbst.GetSortedList();
        }
        public T FindByIdx(int idx)
        {
            return rbst.FindByIdx(idx);
        }
        public int Count()
        {
            return rbst.GetTreeSize();
        }
        public T Max()
        {
            return rbst.Max();
        }
        public T Min()
        {
            return rbst.Min();
        }
    }
    /// <summary>
    /// O(logN) で二分探索可能な MultiSet On RBST
    /// ただし定数倍は重い。2*10^5 くらいでギリギリ。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiSetOnRBST<T> : SetOnRBST<T> where T : IComparable
    {
        /// <summary>
        /// 多重集合に要素を加える。SetOnRBST.Add() と違って重複を許容して挿入する。
        /// </summary>
        /// <param name="value"></param>
        /// <returns>要素が既に存在すれば false, 要素が存在しなければ true</returns>
        public override bool Add(T value)
        {
            if (Contains(value))
            {
                rbst.Insert(value);
                return false;
            }
            else
            {
                rbst.Insert(value);
                return true;
            }
        }
    }
    /// <summary>
    /// 二分探索をかけることができる Dictionary。
    /// RBST を利用しており定数倍が重いため、二分探索が不要な場合は System.Collections.Generic の Dictionary を使った方が良い。
    /// ※ 実装がめんどかったので、とりあえず「RBST と Dictionary を両方持つだけ」というしょうもない実装になっている。
    /// ContainsKey, Add, Remove, LowerBound, UpperBound ともに O(logN)。ただし定数倍重し（2*10^5 は 2ms だと厳しい。値の重複があって探索が減ればいけるが……）
    /// ※ 例外処理は全然してないので、エラーを起こさないように使う
    /// </summary>
    /// <typeparam name="TKey">IComparable</typeparam>
    /// <typeparam name="TValue">any</typeparam>
    public class DictionaryOnRBST<TKey, TValue> where TKey : IComparable
    {
        protected RandomizedBinarySearchTree<TKey> rbst;
        protected Dictionary<TKey, TValue> dictionary;

        public DictionaryOnRBST()
        {
            rbst = new RandomizedBinarySearchTree<TKey>();
            dictionary = new Dictionary<TKey, TValue>();
        }
        public bool ContainsKey(TKey key)
        {
            if (dictionary.ContainsKey(key)) return true;
            else return false;
        }
        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                rbst.Insert(key);
                dictionary.Add(key, value);
            }
        }
        /// <summary>
        /// TKey key が存在すれば削除し、対応する value を返す。
        /// 存在しなければ、default(TValue) を返す。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Remove(TKey key)
        {
            if (ContainsKey(key))
            {
                rbst.Remove(key);
                dictionary.Remove(key);
            }
        }
        /// <summary>
        /// 要素へのアクセスは this[key]（インデクサ）
        /// </summary>
        /// <param name="key">TKey</param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                return dictionary[key];
            }
            set
            {
                Add(key, value);
            }
        }
        /// <summary>
        /// key 以上の要素のうち最も左側の添字位置を返す（存在しなければ size を返す）
        /// ex) k-3, k-2, k-1, (k) ←ココ, k, ...
        /// rbst が null のときは 0 を返す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int LowerBound(TKey key)
        {
            return rbst.LowerBound(key);
        }
        /// <summary>
        /// key より大きい要素のうち最も左側の添字位置を返す（存在しなければ size を返す）
        /// ex) k-3, k-2, k-1, k, k, (k+1)←ココ, k+2, ...
        /// rbst が null のときは 0 を返す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int UpperBound(TKey key)
        {
            return rbst.UpperBound(key);
        }
        /// <summary>
        /// key でソートされた KeyValuePair を返す。
        /// 単に rbst.GetSortedList() して それに合わせて dictionary[key] を N 回呼んでいるだけなので、
        /// 少なく見積もっても O(NlogN) くらいはかかっていると思う。あまり使いたくない
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<TKey, TValue>> GetSortedKeyValueList()
        {
            var sortedKeyValueList = new List<KeyValuePair<TKey, TValue>>();
            var sortedKeyList = rbst.GetSortedList();
            foreach (var key in sortedKeyList)
            {
                sortedKeyValueList.Add(new KeyValuePair<TKey, TValue>(key, dictionary[key]));
            }
            return sortedKeyValueList;
        }
        public List<TKey> GetSortedKeyList()
        {
            return rbst.GetSortedList();
        }
    }
    /// <summary>
    /// 二分探索のできる MultiDictionary。value の重複は許される（つまり TKey, MultiSet）
    /// DictionaryOnRBST 同様 RBST を利用しており定数倍が重いが、MultiDictionary は C# に無いので仕方ない。間に合わない場合 C++ を使うか？
    /// TValue にも MultiSetOnRBST を使っているので、なおさら重い。
    /// ContainsKey, Add, Remove, LowerBound, UpperBound ともに O(logN)。ただし定数倍重し（2*10^5 は 2ms だと厳しい。値の重複があって探索が減ればいけるが……）
    /// ※ 例外処理は全然してないので、エラーを起こさないように使う
    /// </summary>
    /// <typeparam name="TKey">IComparable</typeparam>
    /// <typeparam name="TValue">IComparable</typeparam>
    public class MultiDictionaryOnRBST<TKey, TValue>
        where TKey : IComparable
        where TValue : IComparable
    {
        protected RandomizedBinarySearchTree<TKey> rbst;
        protected Dictionary<TKey, MultiSetOnRBST<TValue>> dictionary;
        public MultiDictionaryOnRBST()
        {
            rbst = new RandomizedBinarySearchTree<TKey>();
            dictionary = new Dictionary<TKey, MultiSetOnRBST<TValue>>();
        }
        /// <summary>
        /// 指定する key の存在を確認
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            if (dictionary.ContainsKey(key)) return true;
            else return false;
        }
        /// <summary>
        /// 指定する (key, value) ペアの存在を確認
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsKeyValue(TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key)) return false;
            else if (dictionary[key].Contains(value)) return true;
            else return false;
        }
        /// <summary>
        /// （TKey, TValue) のペアを挿入する。
        /// key, value ともに重複を許すため、同じ (key, value) があった場合も挿入される。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                rbst.Insert(key);
                dictionary[key].Add(value);
            }
            else
            {
                rbst.Insert(key);
                var setAsNewValue = new MultiSetOnRBST<TValue>();
                setAsNewValue.Add(value);
                dictionary.Add(key, setAsNewValue);
            }
        }
        /// <summary>
        /// key に対応する value（＝key, value のペア）の数を返す
        /// </summary>
        /// <param name=""></param>
        public int Count(TKey key)
        {
            if (!ContainsKey(key)) return 0;
            else return dictionary[key].Count();
        }
        /// <summary>
        /// TKey key が存在する場合、その key に対応する value を全て消す。
        /// 存在しなければ何もしない（エラーも発生させない）。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Remove(TKey key)
        {
            int numValue = Count(key);
            while (numValue > 0)
            {
                rbst.Remove(key);
            }
            dictionary.Remove(key);
        }
        /// <summary>
        /// 指定された (key, value) ペアを 1 つ消す（複数ある場合も 1 つしか消さない）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Remove(TKey key, TValue value)
        {
            if (ContainsKeyValue(key, value))
            {
                rbst.Remove(key);
                if (Count(key) == 1)
                {
                    dictionary.Remove(key);
                }
                else
                {
                    dictionary[key].Remove(value);
                }
            }
        }
        /// <summary>
        /// key に対応する value の multiset を返す。O(N)
        /// 代入は不可
        /// </summary>
        /// <param name="key">TKey</param>
        /// <returns></returns>
        public MultiSetOnRBST<TValue> this[TKey key]
        {
            get
            {
                return dictionary[key];
            }
            private set { }
        }
        /// <summary>
        /// key 以上の要素のうち最も左側の添字位置を返す（存在しなければ size を返す）
        /// ex) k-3, k-2, k-1, (k) ←ココ, k, ...
        /// rbst が null のときは 0 を返す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int LowerBound(TKey key)
        {
            return rbst.LowerBound(key);
        }
        /// <summary>
        /// key より大きい要素のうち最も左側の添字位置を返す（存在しなければ size を返す）
        /// ex) k-3, k-2, k-1, k, k, (k+1)←ココ, k+2, ...
        /// rbst が null のときは 0 を返す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int UpperBound(TKey key)
        {
            return rbst.UpperBound(key);
        }
        /// <summary>
        /// key -> value の順で昇順ソートされた KeyValuePair を返す。
        /// 単に rbst.GetSortedList() したあと各 key において dictionary[key](=MultiSetOnRBST).GetSortedList()  を N 回呼んでいるだけなので、
        /// 少なく見積もっても O(NlogN) 以上かかっていると思う。あまり使いたくない
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<TKey, TValue>> GetSortedKeyValueList()
        {
            var sortedKeyValueList = new List<KeyValuePair<TKey, TValue>>();
            var sortedKeyList = rbst.GetSortedList();
            foreach (var key in sortedKeyList)
            {
                var sortedValueList = dictionary[key].GetSortedList();
                foreach (TValue value in sortedValueList)
                {
                    sortedKeyValueList.Add(new KeyValuePair<TKey, TValue>(key, value));
                }
            }
            return sortedKeyValueList;
        }
        public List<TKey> GetSortedKeyList()
        {
            return rbst.GetSortedList();
        }
    }
    /// <summary>
    /// Priority Queue （優先順位付きキュー）
    /// 参考：蟻本 p.69 及び https://yambe2002.hatenablog.com/entry/2015/11/01/114503
    /// 二分木の各ノードに上から順番に 0, 1, 2, ... と番号を付け、それを配列 T[] で保持する
    /// Push(): O(logN), Pop(): O(logN)
    /// IComparer&lt;T&gt; で明示しない限り T.Compare(x, y) で比較する
    /// </summary>
    /// <typeparam name="T">IComparable</typeparam>
    public class PriorityQueue<T> where T : IComparable
    {
        private IComparer<T> comparer = null;
        private bool isDescending = true;

        private T[] binaryHeap;

        private int count = 0;

        /// <summary>
        /// Priority queue
        /// </summary>
        /// <param name="maxSize">max size</param>
        /// <param name="isDescending">Default: true (Descending)</param>
        public PriorityQueue(int maxSize, bool isDescending = true)
        {
            this.isDescending = isDescending;
            binaryHeap = new T[maxSize];
        }

        /// <summary>
        /// Priority Queue with custom comparer
        /// comparer: comparer.compare(x, y) -> returns (x - y)
        /// </summary>
        public PriorityQueue(int maxSize, IComparer<T> comparer, bool isDescending = true)
        {
            this.comparer = comparer;
            this.isDescending = isDescending;
            binaryHeap = new T[maxSize];

        }

        /// <summary>
        /// compare x and y according to the designated rule (comparer & isDescending)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>+ : x is prior to y, 0 : x == y, - : y is prior to x</returns>
        private int Compare(T x, T y)
        {
            if (comparer == null)
            {
                if (isDescending) return x.CompareTo(y);
                else return y.CompareTo(x);
            }
            else
            {
                if (isDescending) return comparer.Compare(x, y);
                else return comparer.Compare(y, x);
            }
        }

        /// <summary>
        /// O(logN)
        /// T[] の末尾に格納しようとするが、親と比べて逆転していたら、ひっくり返す
        /// </summary>
        /// <param name="x"></param>
        public void Push(T x)
        {
            int x_idx = count;

            // 親と比べて逆転していたら、親の値を今の idx に入れ、元親の idx へと登る
            while (x_idx > 0)
            {
                int parent_idx = (x_idx - 1) / 2;

                // 逆転していなければ（親が優先なら）抜ける
                if (Compare(binaryHeap[parent_idx], x) >= 0) break;
                else
                {
                    binaryHeap[x_idx] = binaryHeap[parent_idx];
                    x_idx = parent_idx;
                }
            }
            // 場所が確定したので格納する
            binaryHeap[x_idx] = x;
            count++;
        }

        /// <summary>
        /// O(logN)
        /// 根の値を取り出し、配列の末尾の値を根に格納し、子と比べて逆転していればひっくり返していく
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            T ret = binaryHeap[0];

            // 1. 最後尾のノード x を根に入れる（元の最後尾ノードは放置）
            //    ※実際はすぐに根に代入せず、最終的に収まるべき x_idx を更新していく
            // 2. 逆転している子があれば、逆転度合いの大きい方と交換していく
            T x = binaryHeap[count - 1];
            int x_idx = 0;
            count--;
            // 子がなくなる（自身が葉となる）まで逆転チェック
            while (x_idx * 2 + 1 < count)
            {
                // まずは左側の子を交換候補とする
                int candidate_idx = x_idx * 2 + 1;
                T candidate = binaryHeap[candidate_idx];

                // 右側の子が存在し、右側の子のが優先順位が高ければ、右側の子を候補とする
                int child_r_idx = candidate_idx + 1;
                if (child_r_idx < count && Compare(candidate, binaryHeap[child_r_idx]) < 0)
                {
                    candidate_idx = child_r_idx;
                    candidate = binaryHeap[child_r_idx];
                }

                // 自身と交換候補の比較
                // 候補のが優先順位が高ければ、今の場所に候補を入れ、候補の idx へと下る
                if (Compare(candidate, x) > 0)
                {
                    binaryHeap[x_idx] = candidate;
                    x_idx = candidate_idx;
                }
                else break;
            }
            binaryHeap[x_idx] = x;

            return ret;
        }

        /// <summary>
        /// O(1)
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return count;
        }

        /// <summary>
        /// O(logN)
        /// </summary>
        /// <returns></returns>
        public T Top()
        {
            return binaryHeap[0];
        }

        /// <summary>
        /// O(N) 注意！
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool Contains(T x)
        {
            foreach (T t in binaryHeap)
            {
                if (t.Equals(x)) return true;
            }
            return false;
        }

        public void Clear()
        {
            while (this.Count() > 0) this.Pop();
        }

        /// <summary>
        /// GetEnumerator() を定義しておくことで foreach をかけられる
        /// （IEnumerable にはしなくても良いらしい）
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            List<T> ret = new List<T>();

            while (this.Count() > 0)
            {
                ret.Add(this.Pop());
            }
            foreach (T t in ret)
            {
                this.Push(t);
                yield return t;
            }
        }

        /// <summary>
        /// 優先度順に並べ替えて配列化
        /// 未 verify！！
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] array = new T[count];
            int idx = 0;
            foreach (T x in this) array[idx++] = x;
            return array;
        }
    }
}