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
using DotSpatial.Topology.Geometries;
using Wintellect.PowerCollections;

namespace DotSpatial.Topology.GeometriesGraph
{
    /// <summary>
    /// A map of nodes, indexed by the coordinate of the node.
    /// </summary>
    public class NodeMap
    {
        #region Fields

        private readonly NodeFactory _nodeFact;
        private readonly IDictionary<Coordinate, Node > _nodeMap = new OrderedDictionary<Coordinate, Node>();

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="nodeFact"></param>
        public NodeMap(NodeFactory nodeFact)
        {
            _nodeFact = nodeFact;
        }

        #endregion

        #region Properties

        /// <summary>
        ///
        /// </summary>
        public virtual IList<Node> Values
        {
            get { return new List<Node>(_nodeMap.Values); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a node for the start point of this EdgeEnd
        /// (if one does not already exist in this map).
        /// Adds the EdgeEnd to the (possibly new) node.
        /// </summary>
        /// <param name="e"></param>
        public virtual void Add(EdgeEnd e)
        {
            Coordinate p = e.Coordinate;
            Node n = AddNode(p);
            n.Add(e);
        }

        /// <summary>
        /// This method expects that a node has a coordinate value.
        /// </summary>
        /// <param name="coord"></param>
        public virtual Node AddNode(Coordinate coord)
        {
            Node node;
            if (!_nodeMap.TryGetValue(coord, out node))
            {
                node = _nodeFact.CreateNode(coord);
                _nodeMap.Add(coord, node);
            }
            return node;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public virtual Node AddNode(Node n)
        {
            Node node = _nodeMap[n.Coordinate];
            if (node == null) 
            {
                _nodeMap.Add(n.Coordinate, n);
                return n;
            }
            node.MergeLabel(n);
            return node;
        }

        /// <returns>
        /// The node if found; null otherwise.
        /// </returns>
        /// <param name="coord"></param>
        public virtual Node Find(Coordinate coord)
        {
            Node res;
            if (!_nodeMap.TryGetValue(coord, out res))
                return null;
            return res;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <returns></returns>
        public virtual IList<Node> GetBoundaryNodes(int geomIndex)
        {
            IList<Node> bdyNodes = new List<Node>();
            foreach (Node node in _nodeMap.Values)
            {
                if (node.Label.GetLocation(geomIndex) == LocationType.Boundary)
                    bdyNodes.Add(node);
            }
            return bdyNodes;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Node> GetEnumerator()
        {
            return _nodeMap.Values.GetEnumerator();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="outstream"></param>
        public virtual void Write(StreamWriter outstream)
        {
            foreach (Node node in _nodeMap.Values)
            {
                node.Write(outstream);
            }
        }

        #endregion
    }
}