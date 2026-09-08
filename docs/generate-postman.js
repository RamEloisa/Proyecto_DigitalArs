const fs = require("fs");
const path = require("path");

const openapi = JSON.parse(
    fs.readFileSync(path.join(__dirname, "openapi.json"), "utf8")
);

const outputDir = path.join("docs", "postman");
const outputFile = path.join(
    outputDir,
    "DigitalArs.postman_collection.json"
);

fs.mkdirSync(outputDir, { recursive: true });

const collection = {
    info: {
        name: "DigitalArs API",
        description: "Colección de requests para la API DigitalArs.",
        schema:
            "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
    },

    variable: [
        {
            key: "baseUrl",
            value: "http://localhost:5179"
        },
        {
            key: "token",
            value: ""
        }
    ],

    item: []
};

// ---------------------------------------------------------
// Helpers
// ---------------------------------------------------------

function resolveRef(ref) {
    if (!ref || !ref.startsWith("#/")) {
        return null;
    }

    const parts = ref.substring(2).split("/");
    let current = openapi;

    for (const part of parts) {
        current = current?.[part];
    }

    return current;
}

function resolveSchema(schema) {
    if (!schema) {
        return null;
    }

    if (schema.$ref) {
        return resolveRef(schema.$ref);
    }

    return schema;
}

function generateExample(schema) {
    schema = resolveSchema(schema);

    if (!schema) {
        return {};
    }

    if (schema.example !== undefined) {
        return schema.example;
    }

    if (schema.default !== undefined) {
        return schema.default;
    }

    if (schema.enum?.length) {
        return schema.enum[0];
    }

    if (schema.type === "object" || schema.properties) {
        const result = {};

        for (const [key, property] of Object.entries(
            schema.properties || {}
        )) {
            result[key] = generateExample(property);
        }

        return result;
    }

    if (schema.type === "array") {
        return [generateExample(schema.items)];
    }

    switch (schema.type) {
        case "integer":
        case "number":
            return 0;

        case "boolean":
            return false;

        case "string":
            if (schema.format === "date-time") {
                return new Date().toISOString();
            }

            if (schema.format === "date") {
                return new Date().toISOString().split("T")[0];
            }

            return "";

        default:
            return {};
    }
}

function getRequestBody(operation) {
    const content = operation.requestBody?.content;

    if (!content) {
        return null;
    }

    const jsonContent =
        content["application/json"] ||
        content["application/*+json"] ||
        Object.values(content)[0];

    if (!jsonContent) {
        return null;
    }

    const schema = resolveSchema(jsonContent.schema);

    let example;

    if (jsonContent.example !== undefined) {
        example = jsonContent.example;
    } else {
        example = generateExample(schema);
    }

    return {
        mode: "raw",
        raw: JSON.stringify(example, null, 2),
        options: {
            raw: {
                language: "json"
            }
        }
    };
}

function getParameters(operation, pathParameters) {
    const parameters = [
        ...(pathParameters || []),
        ...(operation.parameters || [])
    ];

    return parameters.map(parameter => {
        const schema = resolveSchema(parameter.schema);

        let value = "";

        if (parameter.example !== undefined) {
            value = String(parameter.example);
        } else if (parameter.example === 0) {
            value = "0";
        } else if (schema?.example !== undefined) {
            value = String(schema.example);
        } else if (parameter.in === "path") {
            value = parameter.name === "id"
                ? "1"
                : `{{${parameter.name}}}`;
        }

        return {
            key: parameter.name,
            value,
            description: parameter.description || undefined
        };
    });
}

function isLoginEndpoint(route, method) {
    return route.toLowerCase() === "/api/auth/login" &&
        method.toLowerCase() === "post";
}

function isProtectedEndpoint(route, operation) {
    if (isLoginEndpoint(route, operation.method || "")) {
        return false;
    }

    return true;
}

function getTag(operation) {
    if (operation.tags?.length) {
        return operation.tags[0];
    }

    return "Other";
}

// ---------------------------------------------------------
// Controllers / folders
// ---------------------------------------------------------

const folders = new Map();

for (const [route, pathDefinition] of Object.entries(openapi.paths || {})) {
    for (const [method, operation] of Object.entries(pathDefinition)) {

        // OpenAPI puede tener campos que no son HTTP methods.
        const httpMethods = [
            "get",
            "post",
            "put",
            "patch",
            "delete",
            "options",
            "head",
            "trace"
        ];

        if (!httpMethods.includes(method.toLowerCase())) {
            continue;
        }

        const tag = getTag(operation);

        if (!folders.has(tag)) {
            folders.set(tag, []);
        }

        const request = {
            name:
                operation.summary ||
                `${method.toUpperCase()} ${route}`,

            request: {
                method: method.toUpperCase(),

                header: [],

                url: {
                    raw:
                        `{{baseUrl}}${route}`,

                    host: [
                        "{{baseUrl}}"
                    ],

                    path: route
                        .split("/")
                        .filter(Boolean)
                        .map(segment =>
                            segment.startsWith("{") && segment.endsWith("}")
                                ? `:${segment.substring(1, segment.length - 1)}`
                                : segment
                        )
                }
            },

            response: []
        };

        const queryParameters = getParameters(
            operation,
            pathDefinition.parameters
        );

        const query = queryParameters.filter(
            parameter =>
                openapi.paths[route] &&
                (
                    operation.parameters?.find(
                        p => p.name === parameter.key
                    )?.in === "query" ||
                    pathDefinition.parameters?.find(
                        p => p.name === parameter.key
                    )?.in === "query"
                )
        );

        if (query.length > 0) {
            request.request.url.query = query.map(parameter => ({
                key: parameter.key,
                value: parameter.value,
                description: parameter.description
            }));
        }

        const body = getRequestBody(operation);

        if (body) {
            request.request.header.push({
                key: "Content-Type",
                value: "application/json"
            });

            request.request.body = body;
        }

        // -------------------------------------------------
        // Authentication
        // -------------------------------------------------

        if (isProtectedEndpoint(route, {
            ...operation,
            method
        })) {
            request.request.auth = {
                type: "bearer",
                bearer: [
                    {
                        key: "token",
                        value: "{{token}}",
                        type: "string"
                    }
                ]
            };
        } else {
            request.request.auth = {
                type: "noauth"
            };
        }

        // -------------------------------------------------
        // Login: guardar automáticamente el JWT
        // -------------------------------------------------

        if (isLoginEndpoint(route, method)) {
            request.event = [
                {
                    listen: "test",
                    script: {
                        type: "text/javascript",
                        exec: [
                            "const json = pm.response.json();",
                            "",
                            "if (json.token) {",
                            "    pm.collectionVariables.set('token', json.token);",
                            "    console.log('JWT guardado correctamente.');",
                            "}"
                        ]
                    }
                }
            ];
        }

        folders.get(tag).push(request);
    }
}

// ---------------------------------------------------------
// Orden de controllers
// ---------------------------------------------------------

const preferredOrder = [
    "Auth",
    "Users",
    "Accounts",
    "Transactions",
    "Notifications",
    "FixedTermDeposits"
];

const sortedFolders = [
    ...preferredOrder.filter(tag => folders.has(tag)),
    ...[...folders.keys()]
        .filter(tag => !preferredOrder.includes(tag))
        .sort()
];

for (const tag of sortedFolders) {
    collection.item.push({
        name: tag,
        item: folders.get(tag)
    });
}

// ---------------------------------------------------------
// Guardar
// ---------------------------------------------------------

fs.writeFileSync(
    outputFile,
    JSON.stringify(collection, null, 2),
    "utf8"
);

console.log("");
console.log("======================================");
console.log(" DigitalArs Postman Collection");
console.log("======================================");
console.log("");
console.log(`OpenAPI: ${openapi.openapi}`);
console.log(`Endpoints: ${Object.keys(openapi.paths || {}).length}`);
console.log(`Archivo generado: ${outputFile}`);
console.log("");