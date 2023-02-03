import React, { FunctionComponent, useState, useMemo } from "react";

import { Logo } from "./Logo";
import { MenuIcon, MenuCloseIcon } from "./icons";

interface IAsideMenuProps {
    children: any;
}

const asideMenu: FunctionComponent<IAsideMenuProps> = ({ children }: IAsideMenuProps) => {
    const [isOpen, setIsOpen] = useState(true);

    const logoSize = useMemo(() => isOpen ? 56 : 32, [isOpen]);

    return (
        <aside className={`acm-aside ${isOpen ? "is-open" : ""}`}>
            <button
                title={isOpen ? "close menu" : "open menu"}
                className="acm-aside__open-btn"
                onClick={() => setIsOpen(!isOpen)}>
                {isOpen ? <MenuCloseIcon /> : <MenuIcon />}
            </button>
            <Logo size={logoSize} />
            <nav className="acm-aside-menu">{children}</nav>
        </aside>
    );
};

export { asideMenu as AsideMenu };
