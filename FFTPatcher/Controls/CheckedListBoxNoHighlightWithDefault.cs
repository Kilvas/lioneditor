﻿/*
    Copyright 2007, Joe Davidson <joedavidson@gmail.com>

    This file is part of FFTPatcher.

    FFTPatcher is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    FFTPatcher is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with FFTPatcher.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace FFTPatcher.Controls
{
    /// <summary>
    /// Represents a <see cref="CheckedListBox"/> that allows default values to be specified.
    /// </summary>
    public partial class CheckedListBoxNoHighlightWithDefault : CheckedListBox
    {
        private SortedList haveOutlines = new SortedList( null as IComparer );

        private bool[] defaults;

        /// <summary>
        /// Gets the default values for the items in this <see cref="CheckedListBoxNoHighlightWithDefault"/>.
        /// </summary>
        public bool[] Defaults
        {
            get { return defaults; }
            private set { defaults = value; }
        }

        /// <summary>
        /// Sets a list of values and their defaults.
        /// </summary>
        public void SetValuesAndDefaults( bool[] values, bool[] defaults )
        {
            if( (values != null) && (defaults != null) && (this.defaults == null) )
            {
                this.defaults = defaults;
                for( int i = 0; i < Items.Count; i++ )
                {
                    SetItemChecked( i, values[i] );
                    RefreshItem( i );
                }
            }
            else if( (values != null) && (defaults != null) && (this.defaults != null) )
            {
                List<int> itemsToRefresh = new List<int>( values.Length );
                for( int i = 0; i < Items.Count; i++ )
                {
                    if( ((GetItemChecked( i ) ^ this.defaults[i]) && !(values[i] ^ defaults[i])) ||
                        (!(GetItemChecked( i ) ^ this.defaults[i]) && (values[i] ^ defaults[i])) )
                    {
                        itemsToRefresh.Add( i );
                    }
                }

                this.defaults = defaults;
                for( int i = 0; i < Items.Count; i++ )
                {
                    SetItemChecked( i, values[i] );
                }

                foreach( int i in itemsToRefresh )
                {
                    SetItemChecked( i, !values[i] );
                    SetItemChecked( i, values[i] );
                }
            }
        }

        public CheckedListBoxNoHighlightWithDefault()
        {
            this.CheckOnClick = true;
        }

        protected override void OnItemCheck( ItemCheckEventArgs e )
        {
            RefreshItem( e.Index );
            base.OnItemCheck( e );
        }

        protected override void OnDrawItem( DrawItemEventArgs e )
        {
            Brush backColorBrush = new SolidBrush( this.BackColor );
            Brush foreColorBrush = new SolidBrush( this.ForeColor );

            e.Graphics.FillRectangle( backColorBrush, e.Bounds );
            if( e.Index < Items.Count )
            {
                CheckBoxState state = this.GetItemChecked( e.Index ) ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
                Size checkBoxSize = CheckBoxRenderer.GetGlyphSize( e.Graphics, state );
                Point loc = new Point( 1, (e.Bounds.Height - (checkBoxSize.Height + 1)) / 2 + 1 );
                CheckBoxRenderer.DrawCheckBox( e.Graphics, new Point( loc.X + e.Bounds.X, loc.Y + e.Bounds.Y ), state );
                e.Graphics.DrawString( this.Items[e.Index].ToString(), e.Font, foreColorBrush, new PointF( loc.X + checkBoxSize.Width + 1 + e.Bounds.X, loc.Y + e.Bounds.Y ) );

                if( (Defaults != null) && (Defaults.Length > e.Index) && (Defaults[e.Index] != GetItemChecked( e.Index )) )
                {
                    using( Pen p = new Pen( Color.Blue, 1 ) )
                    {
                        e.Graphics.DrawRectangle( p, new Rectangle( e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1 ) );
                        if( !haveOutlines.Contains( e.Index ) )
                        {
                            haveOutlines.Add( e.Index, null );
                        }
                    }
                }
            }
        }
    }
}