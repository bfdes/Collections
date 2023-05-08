package in.bfdes.collections;

import java.util.Iterator;
import java.util.NoSuchElementException;
import java.util.function.Function;

public class BinarySearchTree<K extends Comparable<K>, V> implements Tree<K, V> {
    private Node root;

    private class Node {
        public K key;
        public V value;
        public Node left, right;
        /*
        Size of the tree rooted at this node.
         */
        public int size = 1;

        public Node(K key, V value) {
            this.key = key;
            this.value = value;
        }

        public Node min() {
            var node = this;
            while (node.left != null)
                node = node.left;
            return node;
        }

        public Node deleteMin() {
            if (left == null)
                return right;
            left = left.deleteMin();
            size = 1 + size(left) + size(right);
            return this;
        }

        public boolean isSymmetric() {
            var isLeftSymmetric =
                    left == null || key.compareTo(left.key) > 0 && left.isSymmetric();
            var isRightSymmetric =
                    right == null || key.compareTo(right.key) < 0 && right.isSymmetric();
            return isLeftSymmetric && isRightSymmetric;
        }
    }

    @Override
    public void put(K key, V value) {
        if (key == null || value == null)
            throw new IllegalArgumentException();

        var put = new Function<Node, Node>() {
            @Override
            public Node apply(Node node) {
                if (node == null)
                    return new Node(key, value);
                var diff = key.compareTo(node.key);
                if (diff == 0)
                    node.value = value;
                else if (diff < 0)
                    node.left = apply(node.left);
                else
                    node.right = apply(node.right);
                node.size = 1+ size(node.left) + size(node.right);
                return node;
            }
        };

        root = put.apply(root);
    }

    @Override
    public V get(K key) {
        if (key == null)
            throw new IllegalArgumentException();

        var node = root;

        while (node != null) {
            var diff = key.compareTo(node.key);
            if (diff == 0)
                break;
            else if (diff < 0)
                node = node.left;
            else
                node = node.right;
        }
        if (node == null)
            throw new NoSuchElementException();
        return node.value;
    }

    @Override
    public V getOrElse(K key, V defaultValue) {
        try {
            return get(key);
        } catch (NoSuchElementException ignored) {
            return defaultValue;
        }
    }

    @Override
    public void delete(K key) {
        if (key == null)
            throw new IllegalArgumentException();

        var delete = new Function<Node, Node>() {
            @Override
            public Node apply(Node node) {
                if (node == null)
                    return null;

                var diff = key.compareTo(node.key);

                if (diff < 0)
                    node.left = apply(node.left);
                else if (diff > 0)
                    node.right = apply(node.right);
                else {
                    if (node.left == null) return node.right;
                    if (node.right == null) return node.left;
                    var min = node.right.min();
                    node.key = min.key;
                    node.value = min.value;
                    node.right = node.right.deleteMin();
                }
                node.size = 1 + size(node.left) + size(node.right);
                return node;
            }
        };

        root = delete.apply(root);
    }

    @Override
    public boolean contains(K key) {
        try {
            get(key);
            return true;
        } catch (NoSuchElementException ignored) {
            return false;
        }
    }

    @Override
    public boolean isEmpty() {
        return root == null;
    }

    @Override
    public int size() {
        return size(root);
    }

    @Override
    public Iterator<Pair<K, V>> iterator() {
        return new Iterator<>() {
            private final List<Node> stack = new LinkedList<>();
            private Node node = root;

            @Override
            public boolean hasNext() {
                return node != null || !stack.isEmpty();
            }

            @Override
            public Pair<K, V> next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                while (node != null) {
                    stack.push(node);
                    node = node.left;
                }
                node = stack.pop();
                var tuple = new Pair<>(node.key, node.value);
                node = node.right;
                return tuple;
            }
        };
    }

    @Override
    public Iterable<K> keys() {
        return () -> new Iterator<>() {
            private final Iterator<Pair<K, V>> items = iterator();

            @Override
            public boolean hasNext() {
                return items.hasNext();
            }

            @Override
            public K next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                return items.next().key();
            }
        };
    }

    @Override
    public Iterable<V> values() {
        return () -> new Iterator<>() {
            private final Iterator<Pair<K, V>> items = iterator();

            @Override
            public boolean hasNext() {
                return items.hasNext();
            }

            @Override
            public V next() {
                if (!hasNext())
                    throw new NoSuchElementException();
                return items.next().value();
            }
        };
    }

    @Override
    public K min() {
        if (isEmpty())
            throw new IllegalStateException();
        return root.min().key;
    }

    @Override
    public K max() {
        if (isEmpty())
            throw new IllegalStateException();

        var node = root;

        while (node.right != null)
            node = node.right;
        return node.key;
    }

    @Override
    public K floor(K key) {
        if (key == null)
            throw new IllegalArgumentException();

        var node = root;
        K floor = null;

        while (node != null) {
            var diff = key.compareTo(node.key);
            if (diff == 0) {
                floor = key;
                break;
            } else if (diff > 0) {
                floor = node.key;
                node = node.right;
            } else
                node = node.left;
        }
        if (floor == null)
            throw new IllegalStateException();
        return floor;
    }

    @Override
    public K ceiling(K key) {
        if (key == null)
            throw new IllegalArgumentException();

        var node = root;
        K ceiling = null;

        while (node != null) {
            var diff = key.compareTo(node.key);
            if (diff == 0) {
                ceiling = key;
                break;
            } else if (diff < 0) {
                ceiling = node.key;
                node = node.left;
            } else
                node = node.right;
        }
        if (ceiling == null)
            throw new IllegalStateException();
        return ceiling;
    }

    @Override
    public int rank(K key) {
        if (key == null)
            throw new IllegalArgumentException();
        var rank = 0;
        var node = root;
        while (node != null) {
            var diff = key.compareTo(node.key);
            if (diff == 0) {
                rank += size(node.left);
                break;
            } else if (diff < 0)
                node = node.left;
            else {
                rank += 1 + size(node.left);
                node = node.right;
            }
        }
        return rank;
    }

    /**
     * Returns `true` if this binary tree is in symmetric order, `false` otherwise.
     * Note: A `BinarySearchTree` should always be in symmetric order -- this method is for debugging.
     */
    public boolean isSymmetric() {
        return isEmpty() || root.isSymmetric();
    }

    private int size(Node node) {
        return node == null ? 0 : node.size;
    }
}
