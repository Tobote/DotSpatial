// ********************************************************************************************************
// Product Name: DotSpatial.Topology.dll
// Description:  The basic topology module for the new dotSpatial libraries
// ********************************************************************************************************
// The contents of this file are subject to the Lesser GNU Public License (LGPL)
// you may not use this file except in compliance with the License. You may obtain a copy of the License at
// http://dotspatial.codeplex.com/license  Alternately, you can access an earlier version of this content from
// the Net Topology Suite, which is also protected by the GNU Lesser Public License and the sourcecode
// for the Net Topology Suite can be obtained here: http://sourceforge.net/projects/nts.
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
// ANY KIND, either expressed or implied. See the License for the specific language governing rights and
// limitations under the License.
//
// The Original Code is from the Net Topology Suite, which is a C# port of the Java Topology Suite.
//
// The Initial Developer to integrate this code into MapWindow 6.0 is Ted Dunsford.
//
// Contributor(s): (Open source contributors should list themselves and their modifications here).
// |         Name         |    Date    |                              Comment
// |----------------------|------------|------------------------------------------------------------
// |                      |            |
// ********************************************************************************************************

using System.Collections.Generic;
using System.IO;
using DotSpatial.Topology.Algorithm;
using DotSpatial.Topology.Geometries;

namespace DotSpatial.Topology.GeometriesGraph
{
    /// <summary>
    /// The computation of the <c>IntersectionMatrix</c> relies on the use of a structure
    /// called a "topology graph". The topology graph contains nodes and edges
    /// corresponding to the nodes and line segments of a <c>Geometry</c>. Each
    /// node and edge in the graph is labeled with its topological location relative to
    /// the source point.
    /// Note that there is no requirement that points of self-intersection be a vertex.
    /// Thus to obtain a correct topology graph, <c>Geometry</c>s must be
    /// self-noded before constructing their graphs.
    /// Two fundamental operations are supported by topology graphs:
    /// Computing the intersections between all the edges and nodes of a single graph
    /// Computing the intersections between the edges and nodes of two different graphs
    /// </summary>
    public class PlanarGraph
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        protected IList<EdgeEnd> EdgeEndList = new List<EdgeEnd>();

        /// <summary>
        /// 
        /// </summary>
        private readonly List<Edge> _edges = new List<Edge>();

        /// <summary>
        /// 
        /// </summary>
        private readonly NodeMap _nodes;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of a Planar Graph
        /// </summary>
        /// <param name="nodeFact">A node Factory</param>
        public PlanarGraph(NodeFactory nodeFact)
        {
            _nodes = new NodeMap(nodeFact);
        }

        /// <summary>
        /// Creates a new instance of a Planar Graph using a default NodeFactory
        /// </summary>
        public PlanarGraph()
        {
            _nodes = new NodeMap(new NodeFactory());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of edge ends
        /// </summary>
        public virtual IList<EdgeEnd> EdgeEnds
        {
            get
            {
                return EdgeEndList;
            }
        }

        /// <summary>
        /// Gets or sets the list of edges.
        /// </summary>
        protected internal IList<Edge> Edges
        {
            get { return _edges; }
        }

        /// <summary>
        /// Gets or sets the NodeMap for this graph
        /// </summary>
        protected NodeMap NodeMap
        {
            get { return _nodes; }
        }

        /// <summary>
        /// Gets a list of the actual values contained in the nodes
        /// </summary>
        public IList<Node> Nodes
        {
            get
            {
                return new List<Node>(_nodes.Values);
            }
            //protected set { nodes = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a new EdgeEnd to the planar graph
        /// </summary>
        /// <param name="e">The EdgeEnd to add</param>
        public virtual void Add(EdgeEnd e)
        {
            _nodes.Add(e);
            EdgeEndList.Add(e);
        }

        /// <summary>
        /// Add a set of edges to the graph.  For each edge two DirectedEdges
        /// will be created.  DirectedEdges are NOT linked by this method.
        /// </summary>
        /// <param name="edgesToAdd"></param>
        public void AddEdges(IList<Edge> edgesToAdd)
        {
            // create all the nodes for the edges
            foreach (Edge e in edgesToAdd)
            {
                _edges.Add(e);

                DirectedEdge de1 = new DirectedEdge(e, true);
                DirectedEdge de2 = new DirectedEdge(e, false);
                de1.Sym = de2;
                de2.Sym = de1;

                Add(de1);
                Add(de2);
            }
        }

        /// <summary>
        /// Adds the specified node to the geometry graph's NodeMap
        /// </summary>
        /// <param name="node">The node to add</param>
        /// <returns>The node after the addition</returns>
        public virtual Node AddNode(Node node)
        {
            return _nodes.AddNode(node);
        }

        /// <summary>
        /// Adds a new ICoordinate as though it were a Node to the node map
        /// </summary>
        /// <param name="coord">An ICoordinate to add</param>
        /// <returns>The newly added node</returns>
        public virtual Node AddNode(Coordinate coord)
        {
            return _nodes.AddNode(coord);
        }

        /// <returns>
        /// The node if found; null otherwise
        /// </returns>
        /// <param name="coord"></param>
        public virtual Node Find(Coordinate coord)
        {
            return _nodes.Find(coord);
        }

        /// <summary>
        /// Returns the edge whose first two coordinates are p0 and p1.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <returns> The edge, if found <c>null</c> if the edge was not found.</returns>
        public virtual Edge FindEdge(Coordinate p0, Coordinate p1)
        {
            for (int i = 0; i < _edges.Count; i++)
            {
                Edge e = _edges[i];
                IList<Coordinate> eCoord = e.Coordinates;
                if (p0.Equals(eCoord[0]) && p1.Equals(eCoord[1]))
                    return e;
            }
            return null;
        }

        /// <summary>
        /// Returns the EdgeEnd which has edge e as its base edge
        /// (MD 18 Feb 2002 - this should return a pair of edges).
        /// </summary>
        /// <param name="e"></param>
        /// <returns> The edge, if found <c>null</c> if the edge was not found.</returns>
        public virtual EdgeEnd FindEdgeEnd(Edge e)
        {
            foreach (EdgeEnd ee in EdgeEndList       )
                if (ee.Edge == e) return ee;
            return null;
        }

        /// <summary>
        /// Returns the edge which starts at p0 and whose first segment is
        /// parallel to p1.
        /// </summary>
        /// <param name="p0"></param>
        ///<param name="p1"></param>
        /// <returns> The edge, if found <c>null</c> if the edge was not found.</returns>
        public virtual Edge FindEdgeInSameDirection(Coordinate p0, Coordinate p1)
        {
            for (int i = 0; i < _edges.Count; i++)
            {
                Edge e = _edges[i];
                IList<Coordinate> eCoord = e.Coordinates;
                if (MatchInSameDirection(p0, p1, eCoord[0], eCoord[1]))
                    return e;
                if (MatchInSameDirection(p0, p1, eCoord[eCoord.Count - 1], eCoord[eCoord.Count - 2]))
                    return e;
            }
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Edge> GetEdgeEnumerator()
        {
            return _edges.GetEnumerator();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Node> GetNodeEnumerator()
        {            
            return _nodes.GetEnumerator();         
        }

        /// <summary>
        /// Adds a new EdgeEnd to the planar graph
        /// </summary>
        /// <param name="e"></param>
        protected virtual void InsertEdge(Edge e)
        {
            _edges.Add(e);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <param name="coord"></param>
        /// <returns></returns>
        public virtual bool IsBoundaryNode(int geomIndex, Coordinate coord)
        {
            Node node = _nodes.Find(coord);
            if (node == null)
                return false;
            Label label = node.Label;
            if (label != null && label.GetLocation(geomIndex) == LocationType.Boundary)
                return true;
            return false;
        }

        /// <summary>
        /// Link the DirectedEdges at the nodes of the graph.
        /// This allows clients to link only a subset of nodes in the graph, for
        /// efficiency (because they know that only a subset is of interest).
        /// </summary>
        public virtual void LinkAllDirectedEdges()
        {
            foreach (Node node in Nodes)
                ((DirectedEdgeStar) node.Edges).LinkAllDirectedEdges();
        }

        /// <summary>
        /// For nodes in the Collection, link the DirectedEdges at the node that are in the result.
        /// This allows clients to link only a subset of nodes in the graph, for
        /// efficiency (because they know that only a subset is of interest).
        /// </summary>
        /// <param name="nodes"></param>
        public static void LinkResultDirectedEdges(IList<Node> nodes)
        {
            foreach (Node node in nodes)
                ((DirectedEdgeStar) node.Edges).LinkResultDirectedEdges();
        }

        /// <summary>
        /// Link the DirectedEdges at the nodes of the graph.
        /// This allows clients to link only a subset of nodes in the graph, for
        /// efficiency (because they know that only a subset is of interest).
        /// </summary>
        public virtual void LinkResultDirectedEdges()
        {
            foreach (Node node in Nodes)
                ((DirectedEdgeStar) node.Edges).LinkResultDirectedEdges();
        }

        /// <summary>
        /// The coordinate pairs match if they define line segments lying in the same direction.
        /// E.g. the segments are parallel and in the same quadrant
        /// (as opposed to parallel and opposite!).
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="ep0"></param>
        /// <param name="ep1"></param>
        private static bool MatchInSameDirection(Coordinate p0, Coordinate p1, Coordinate ep0, Coordinate ep1)
        {
            if (!p0.Equals(ep0))
                return false;
            return CgAlgorithms.ComputeOrientation(p0, p1, ep1) == CgAlgorithms.Collinear &&
                   QuadrantOp.Quadrant(p0, p1) == QuadrantOp.Quadrant(ep0, ep1);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="outstream"></param>
        public virtual void WriteEdges(StreamWriter outstream)
        {
            outstream.WriteLine("Edges:");
            for (int i = 0; i < _edges.Count; i++)
            {
                outstream.WriteLine("edge " + i + ":");
                Edge e = _edges[i];
                e.Write(outstream);
                e.EdgeIntersectionList.Write(outstream);
            }
        }

        #endregion
    }
}