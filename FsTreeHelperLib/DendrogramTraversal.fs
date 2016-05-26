namespace FsTreeHelperLib

/// Module with helper functions for traversing the dendrogram
module DendrogramTraversal =

    open System;

    /// A dendrogram cluster type
    type private ClusterP<'c> = 
      { Cluster : 'c
        ChildL : ClusterP<'c> option
        ChildR : ClusterP<'c> option }

    /// Operations with dendrogram clusters
    module private Cluster =
        
        let isLeaf cluster =
            match (cluster.ChildL, cluster.ChildR) with
            | (None, None) -> true
            | _ -> false
    
    /// Function returning a child of current node
    type ClusterChildAccessor<'c> = delegate of 'c -> 'c

    [<AllowNullLiteral>] //allow for better interop with C#
    /// Class representing one cluster in a dendrogram.
    /// For creating an instance, please use static method Create.
    type Cluster<'c when 'c:null> private (clstrP, isLeaf) =

        /// Accesses the private ClusterP instance
        let clstrP : ClusterP<'c> = clstrP
        let isLeaf = isLeaf
            
        /// The original node which this cluster wraps
        member this.Cluster 
            with get () = clstrP.Cluster

        /// Left child of this cluster
        member this.ChildL
            with get () = match clstrP.ChildL with
                            | Some(ch) -> Cluster(ch, Cluster.isLeaf ch)
                            | None -> null

        /// Right child of this cluster
        member this.ChildR
            with get () = match clstrP.ChildR with
                            | Some(ch) -> Cluster(ch, Cluster.isLeaf ch)
                            | None -> null
        
        /// If doesn't have any children, is considered a leaf
        member this.IsLeaf = isLeaf

        /// Creates a new instance of Cluster, without checking the parameters
        static member private CreatePvt(cluster:'c, lChildAccessor:ClusterChildAccessor<'c>, rChildAccessor:ClusterChildAccessor<'c>) =
            let lChildAccess = lChildAccessor.Invoke(cluster)
            let lChild = match lChildAccess with
                            | null -> None
                            | child -> Some({Cluster = child; ChildL = None; ChildR = None;})

            let rChildAccess = rChildAccessor.Invoke(cluster)
            let rChild = match rChildAccess with
                            | null -> None
                            | child -> Some({Cluster = child; ChildL = None; ChildR = None;})

            let clp = {Cluster = cluster; ChildL = lChild; ChildR = rChild}
            let cl = Cluster(clp, Cluster.isLeaf clp)
            cl

        /// Creates a new instance of Cluster. If any of the parameters are null, returns null. Otherwise a new instance
        /// of Clusters
        static member Create(cluster:'c, lChildAccessor:ClusterChildAccessor<'c>, rChildAccessor:ClusterChildAccessor<'c>) =
            if (isNull cluster) || (isNull lChildAccessor) || (isNull rChildAccessor) then
                null
            else
                Cluster.CreatePvt(cluster, lChildAccessor, rChildAccessor)


