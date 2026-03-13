import "./index.css";
import type { ClassKey } from "keycloakify/login";
import DefaultPage from "keycloakify/login/DefaultPage";
import DefaultTemplate from "keycloakify/login/Template";
import { lazy, Suspense } from "react";
import { useI18n } from "./i18n";
import type { KcContext } from "./KcContext";
import CustomTemplate from "./kcTemplate";

const UserProfileFormFields = lazy(
    () => import("keycloakify/login/UserProfileFormFields")
);
const Login = lazy(() => import("./pages/Login"));
const Register = lazy(() => import("./pages/Register"));

const doMakeUserConfirmPassword = true;

export default function KcPage(props: Readonly<{ kcContext: KcContext }>) {
    const { kcContext } = props;

    const { i18n } = useI18n({ kcContext });

    return (
        <Suspense>
            {(() => {
                if (kcContext.pageId === "login.ftl") {
                    return (
                        <Login
                            {...{ kcContext, i18n, classes }}
                            Template={CustomTemplate}
                            doUseDefaultCss={false}
                        />
                    );
                }

                if (kcContext.pageId === "register.ftl") {
                    return (
                        <Register
                            {...{ kcContext, i18n, classes }}
                            Template={CustomTemplate}
                            doUseDefaultCss={false}
                        />
                    );
                }

                // Остальные страницы Keycloak (сброс пароля, ошибки и т.п.)
                // используют дефолтный шаблон
                return (
                    <DefaultPage
                        kcContext={kcContext}
                        i18n={i18n}
                        classes={classes}
                        Template={DefaultTemplate}
                        doUseDefaultCss={true}
                        UserProfileFormFields={UserProfileFormFields}
                        doMakeUserConfirmPassword={doMakeUserConfirmPassword}
                    />
                );
            })()}
        </Suspense>
    );
}

const classes = {} satisfies { [key in ClassKey]?: string };
