package in.bfdes.collections;

import java.util.Objects;
import java.util.function.Function;

@FunctionalInterface
interface TriFunction<T, U, V, R> {
    R apply(T t, U u, V v);

    default <S> TriFunction<T, U, V, S> andThen(Function<? super R, ? extends S> after) {
        Objects.requireNonNull(after);
        return (T t, U u, V v) -> after.apply(apply(t, u, v));
    }
}
