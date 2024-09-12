import * as React from 'react';
import { Media } from 'react-bootstrap';
import './ColumnItem.scss';


interface Props {
    title: string;
    customClassName?: string;
    subtitle?: string;
    rightContent?: JSX.Element;
    selected?: boolean;
    onClick?: () => void;
    icon?: JSX.Element;
    leftContent?: JSX.Element;
    bodyContent?: JSX.Element[];
}

export const ColumnItem: React.SFC<Props> = ({ title, subtitle, icon, rightContent, customClassName, selected, onClick, bodyContent, leftContent }) => {
    let itemClass = selected ? "column-item selected" : "column-item";
    itemClass = customClassName ? itemClass + " " + customClassName : itemClass;
    return <div className={itemClass} onClick={onClick}>
        <Media>
            <Media.Left align="middle">
                {icon}
                {leftContent}
            </Media.Left>
            <Media.Body>
                <Media.Heading>{title}</Media.Heading>
                <p>{subtitle}</p>
                {bodyContent}
            </Media.Body>
            <Media.Right>
                {rightContent}
            </Media.Right>
        </Media>
    </div>
}