# avtoobves

Website for selling products made of stainless steel in Kharkiv, Ukraine.

## Publishing to IIS

When published to IIS, do not forget to add following section to the web.config:

```
<system.webServer>
    <rewrite>
        <rewriteMaps>
            <rewriteMap name="RedirectWwwToNonWww" />
        </rewriteMaps>
        <rules>
            <rule name="RedirectWwwToNonWww" stopProcessing="true">
                <match url="(.*)" />
                <conditions>
                    <add input="{HTTP_HOST}" pattern="^www.avtoobves.com.ua$" />
                </conditions>
                <action type="Redirect" url="https://avtoobves.com.ua/{R:0}" redirectType="Permanent" />
            </rule>
        </rules>
    </rewrite>
</system.webServer>
```

This should come inside the `<configuration></configuration>` section.