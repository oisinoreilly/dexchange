import * as React from 'react';
import { Glyphicon } from 'react-bootstrap';
import './ContextMenu.scss';

export interface IMenuOption {
    label: string;
    handler: Function;
}

interface ContextMenuProps {
    options: IMenuOption[];
    suppressEventPropagation: boolean;
}

export const ContextMenu: React.SFC<ContextMenuProps> = ({ options, suppressEventPropagation }) => {
    return (
        <div className="dropdown" onClick={(e) => {
            //this stop parent div onClick firing
            if (e.stopPropagation && suppressEventPropagation) e.stopPropagation()
        }}>
            <button className="dropdown-toggle option-button"
                type="button"
                id="contextMenu"
                data-toggle="dropdown"
                aria-haspopup="true"
                aria-expanded="false">
                <Glyphicon glyph="option-vertical" />
            </button>
            <ul className="dropdown-menu dropdown-menu-right is-link" aria-labelledby="dLabel">
                {options.map(opt => <li onClick={() => opt.handler()}><a>{opt.label}</a></li>)}
            </ul>
        </div>
    )
}

