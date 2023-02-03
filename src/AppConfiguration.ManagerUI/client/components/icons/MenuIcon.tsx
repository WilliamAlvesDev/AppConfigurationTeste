import React from "react";

const menuIcon = ({ size = 16, color = "#FFF" }: any) => {
    const width = size;
    const height = 16 / 11 * size;

    return (
        <svg width={width} height={height} viewBox="0 0 16 11" xmlns="http://www.w3.org/2000/svg">
            <g fill={color} fill-rule="evenodd">
                <path d="M0 0h16v1H0zM0 5h16v1H0zM0 10h16v1H0z" />
            </g>
        </svg>
    );
};

export { menuIcon as MenuIcon };
