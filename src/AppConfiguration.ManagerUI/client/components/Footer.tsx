import React, { FunctionComponent } from "react";

const footer: FunctionComponent = () => {
    return <div id="footer">
        <span>XP Inc. @ {new Date().getFullYear()}</span>
    </div>;
};

export {footer as Footer}