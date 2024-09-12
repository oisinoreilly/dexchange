import * as React from 'react';
import { NavLink as Link } from 'react-router-dom';
import { Image } from 'react-bootstrap';
export class NavMenu extends React.Component {
    render() {
        return <div className='main-nav'>
            <div className='navbar navbar-inverse'>
                <div className='navbar-header'>
                    <button type='button' className='navbar-toggle' data-toggle='collapse' data-target='.navbar-collapse'>
                        <span className='sr-only'>Toggle navigation</span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                    </button>
                    <Image src="/logo-gold.png" responsive/>
                    <Link to={'/'}><span className='navbar-brand' style={{ marginLeft: '-124px' }}>DocumentationHQ</span></Link>
                </div>
                <div className='clearfix'></div>
                <div className='navbar-collapse collapse'>
                    <ul className='nav navbar-nav'>
                        <li>
                            <Link to={'/'} activeClassName='active'>
                                <span className='glyphicon glyphicon-home'></span> Home
                            </Link>
                        </li>
                    </ul>
                </div>
            </div>
        </div>;
    }
}
//# sourceMappingURL=NavMenu.jsx.map