namespace CollectionsTest

type Histogram<'k when 'k: comparison>(keys: array<'k>) = 
     member private _.Underlying = (Array.fold(fun map key ->
            let count = 1 + (Option.defaultValue 0 (Map.tryFind key map))
            Map.add key count map
        ) Map.empty keys)
        
     override this.GetHashCode() =
        hash this.Underlying

     override this.Equals(that) = 
        match that with
            | :? Histogram<'k> as h -> this.Underlying.Equals(h.Underlying) 
            | _ -> false
